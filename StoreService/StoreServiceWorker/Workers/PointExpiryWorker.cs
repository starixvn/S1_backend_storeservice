using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;

namespace StoreServiceWorker.Workers;

public class PointExpiryWorker : BackgroundService
{
    private readonly PointSettingBLL _pointSettingbll;
    private readonly RewardPointBLL _rewardPointbll;
    private readonly MemberBLL _memberBll;
    private readonly ILogger<PointExpiryWorker> _logger;
    private readonly IEnumerable<SupportSetting> _supportSetting;
    private readonly IWebHostEnvironment _env;
    private Timer _timer;

    public PointExpiryWorker(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        _supportSetting = configuration.GetSection("SupportSetting").Get<IEnumerable<SupportSetting>>();
        _pointSettingbll = provider.GetRequiredService<PointSettingBLL>();
        _rewardPointbll = provider.GetRequiredService<RewardPointBLL>();
        _memberBll = provider.GetRequiredService<MemberBLL>();
        _logger = provider.GetRequiredService<ILogger<PointExpiryWorker>>();
        _env = provider.GetRequiredService<IWebHostEnvironment>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);

        return Task.CompletedTask;
    }

    private async void DoTaskAsync(object state)
    {
        using (LogContext.PushProperty("WorkerName", "PointExpiryWorker"))
        using (LogContext.PushProperty("TaskId", Guid.NewGuid().ToString()))
        {
            try
            {
                // STEP1: 取得歸零膏設定
                var settings = await _pointSettingbll.GetAllInactivePointExpirySettingsAsync();

                if (!settings.Any())
                {
                    _logger.LogError("GetAllInactivePointExpirySettings Failed.");
                }

                var enableSettings =
                    settings.Where(x => x.Status == (int) GeneralStatusEnum.Enabled);
                
                // STEP2: 取得非活躍用戶
                var inactiveMemberList = new List<CustomerInfo>();
                
                foreach (var setting in enableSettings)
                {
                    var supportCurrencyIds =
                        _supportSetting.First(x => x.SiteId == setting.SiteId).CurrencyIds;

                    foreach (var currencyId in supportCurrencyIds)
                    {
                        
                        var inactiveList = await _memberBll.GetInactiveMemberListAsync(
                            new GetInactiveMemberListParams
                            {
                                SiteId = setting.SiteId,
                                CurrencyId = currencyId,
                                Days = setting.Days
                            });
                        
                        inactiveMemberList.AddRange(inactiveList);
                    }
                }
                
                // STEP3: 點數歸零膏
                var success = await _rewardPointbll.SetPointExpiryAsync(inactiveMemberList);

                if (!success)
                {
                    _logger.LogError("SetPointExpiry Failed." + JsonConvert.SerializeObject(inactiveMemberList));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PointExpiryWorker Exception");
            }
            finally
            {
                var envName = _env.EnvironmentName;
                var ts = envName is "Development" or "Local"
                    ? TimeSpan.FromMinutes(10)
                    : TimeSpan.FromDays(1);
                
                _timer.Change(ts, TimeSpan.Zero);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}