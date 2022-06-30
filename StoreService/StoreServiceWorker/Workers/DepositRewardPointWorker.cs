using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreServiceWorker.Workers;

public class DepositRewardPointWorker : BackgroundService
{
    private readonly RewardPointBLL _rewardPointbll;
    private readonly ILogger<DepositRewardPointWorker> _logger;
    private Timer _timer;

    public DepositRewardPointWorker(IServiceProvider provider)
    {
        _rewardPointbll = provider.GetRequiredService<RewardPointBLL>();
        _logger = provider.GetRequiredService<ILogger<DepositRewardPointWorker>>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);

        return Task.CompletedTask;
    }

    private async void DoTaskAsync(object state)
    {
        try
        {
            var queue = await _rewardPointbll.GetDepositRewardPointQueueAsync();

            if (!queue.Any())
            {
                _logger.LogInformation("No data.");
                return;
            }

            var settings = await _rewardPointbll.GetAllDepositPointSettingAsync();

            if (!settings.Any())
            {
                _logger.LogError("GetAllDepositPointSetting Failed.");
                return;
            }

            //var groupSetting = settings.GroupBy(x => x.SiteId);

            var groupQueue = queue.GroupBy(x => x.CurrencyId);
            foreach (var group in groupQueue)
            {
                var currencyId = group.Key;
                try
                {
                    var depositQueueInfos = new List<DepositRewardPointQueueInfo>();
                    var depositPointInfos = new List<DepositRewardPointInfo>();
                    var groupByCustQueue = group.GroupBy(x => x.CustId);
                    foreach (var custQueue in groupByCustQueue)
                    {
                        var siteId = custQueue.First().SiteId;
                        var setting = settings.First(x => x.SiteId == siteId && x.CurrencyId == currencyId);
                        var pointRate = GetPointRate(setting);
                        var point = Math.Floor(custQueue.Sum(x => x.RemainAmount) / pointRate);
                    }

                    var input = new DepositBatchRewardPointParams
                    {
                        CurrencyId = group.Key,
                        DepositQueueInfos = depositQueueInfos,
                        DepositRewardPointInfos = depositPointInfos
                    };

                    var success = await _rewardPointbll.DepositBatchRewardPointAsync(input);
                    if (!success)
                    {
                        var failedIds = input.DepositQueueInfos.Select(x => x.TransId);
                        _logger.LogError($"Deposit reward point failed, TransIds: {string.Join(',', failedIds)}");
                    }
                }
                catch (Exception ex)
                {
                    var failedIds = group.Select(x => x.TransId);
                    _logger.LogError(
                        $"Deposit reward point failed, Currency: {group.Key}, TransIds: {string.Join(',', failedIds)}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DepositRewardPointWorker Exception");
        }
        finally
        {
            _timer.Change(TimeSpan.FromMinutes(5), TimeSpan.Zero);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private decimal GetPointRate(DepositPointSetting setting)
    {
        if (!setting.PromotionStatus)
            return setting.PointRate;

        var now = DateTimeOffset.Now;

        return now >= setting.PromotionStartTime && now < setting.PromotionEndTime
            ? setting.PromotionPointRate
            : setting.PointRate;
    }
}