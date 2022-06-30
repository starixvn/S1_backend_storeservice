using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model.Enums;
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
                        
                        if (custQueue.Sum(x => x.RemainAmount) / setting.DepositAmount > 0)
                        {
                            var updateTime = custQueue.Max(x => x.DepositUpdateTime);
                            var pointRate = GetPointRate(setting, updateTime);
                            var mergeAmount = 0m;
                            var mergeTransIds = new List<string>();
                            foreach (var q in custQueue.OrderBy(x=>x.TransId))
                            {
                                var amount = mergeAmount + q.RemainAmount;
                                var origPoint = Math.Floor(amount / pointRate);
                                
                                if (origPoint > 0)
                                {
                                    var memo = mergeTransIds.Any()
                                        ? $": merged by {string.Join(',', mergeTransIds)}"
                                        : string.Empty;
                                    
                                    // 清空
                                    mergeAmount = 0;
                                    mergeTransIds = new List<string>();
                                
                                    depositPointInfos.Add(new DepositRewardPointInfo
                                    {
                                        Memo = memo,
                                        Point = origPoint * pointRate,
                                        CustId = custQueue.Key,
                                        SiteId = siteId,
                                        TransId = q.TransId
                                    });
                                    
                                    var remainAmount = q.RemainAmount - (origPoint * setting.DepositAmount);
                                    if (remainAmount > 0)
                                    {
                                        mergeAmount += remainAmount;
                                        mergeTransIds.Add(q.TransId);
                                    }
                                    depositQueueInfos.Add(new DepositRewardPointQueueInfo
                                    {
                                        Status = (int) GetStatus(remainAmount),
                                        RemainAmount = remainAmount > 0 ? remainAmount : 0,
                                        TransId = q.TransId
                                    });
                                }
                            }
                        }
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

    private DepositRewardPointQueueStatusEnum GetStatus(decimal remainAmount)
        => remainAmount > 0
            ? DepositRewardPointQueueStatusEnum.Running
            : DepositRewardPointQueueStatusEnum.Finish;
    
    

    private decimal GetPointRate(DepositPointSetting setting, DateTimeOffset updateTime)
    {
        if (!setting.PromotionStatus)
            return setting.PointRate;
        
        return updateTime >= setting.PromotionStartTime && updateTime < setting.PromotionEndTime
            ? setting.PromotionPointRate
            : setting.PointRate;
    }
}