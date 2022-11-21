using Newtonsoft.Json;
using StoreHangfireService.Job;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;

namespace StoreHangfireService.Service.Expired;

public class PointExpiryService
{
    private readonly PointSettingBLL _pointSettingbll;
    private readonly RewardPointBLL _rewardPointbll;
    private readonly MemberBLL _memberBll;
    private readonly ILogger<BaseJob> _logger;

    public PointExpiryService(IServiceProvider provider)
    {
        _pointSettingbll = provider.GetRequiredService<PointSettingBLL>();
        _rewardPointbll = provider.GetRequiredService<RewardPointBLL>();
        _memberBll = provider.GetRequiredService<MemberBLL>();
        _logger = provider.GetRequiredService<ILogger<BaseJob>>();
    }

    public async Task RunJobAsync()
    {
        // STEP1: 取得歸零膏設定
        var settings = await _pointSettingbll.GetAllInactivePointExpirySettingsAsync();

        if (!settings.Any())
        {
            _logger.LogInformation("Inactive point expiry setting has no data.");
            return ;
        }

        var enableSettings =
                   settings.Where(x => x.Status == (int)GeneralStatusEnum.Enabled);

        if (!enableSettings.Any())
        {
            _logger.LogInformation("Has no enabled inactive point expiry settings.");
            return ;
        }

        // STEP2: 取得非活躍用戶
        var inactiveMemberList = new List<CustomerInfo>();

        foreach (var setting in enableSettings)
        {
            var inactiveList = await _memberBll.GetInactiveMemberListAsync(
                new GetInactiveMemberListParams
                {
                    SiteId = setting.SiteId,
                    Days = setting.Days
                });

            inactiveMemberList.AddRange(inactiveList);
        }

        if (!inactiveMemberList.Any())
        {
            _logger.LogInformation("Has no inactive members.");
            return;
        }

        // STEP3: 點數歸零膏
        var success = await _rewardPointbll.SetPointExpiryAsync(inactiveMemberList);

        if (!success)
        {
            _logger.LogError("SetPointExpiry Failed." + JsonConvert.SerializeObject(inactiveMemberList));
        }

        await Task.CompletedTask;
    }
}
