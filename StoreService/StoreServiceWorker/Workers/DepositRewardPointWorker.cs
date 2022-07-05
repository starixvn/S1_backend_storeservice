using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            // STEP1: 取得待處理佇列
            var queue = await _rewardPointbll.GetDepositRewardPointQueueAsync();

            if (!queue.Any())
            {
                _logger.LogInformation("No data.");
                return;
            }
            
            // STEP2: 取得存款點數設定
            var settings = await _rewardPointbll.GetAllDepositPointSettingAsync();

            if (!settings.Any())
            {
                _logger.LogError("GetAllDepositPointSetting Failed.");
                return;
            }

            // STEP3: 計算點數
            var groupQueue = queue.GroupBy(x => x.CurrencyId);
            foreach (var group in groupQueue)
            {
                _logger.LogInformation("Queue: " +
                    JsonConvert.SerializeObject(group.Select(x => new {x.TransId, x.RemainAmount})));
                
                var currencyId = group.Key;
                var depositQueueInfos = new List<DepositRewardPointQueueInfo>();
                var depositPointInfos = new List<DepositRewardPointInfo>();
                
                try
                {
                    var groupByCustQueue = group.GroupBy(x => x.CustId);
                    
                    foreach (var custQueue in groupByCustQueue)
                    {
                        var siteId = custQueue.First().SiteId;
                        var setting = settings.First(x => x.SiteId == siteId && x.CurrencyId == currencyId);
                        var sumRemainAmount = custQueue.Sum(x => x.RemainAmount);
                        var settingAmount = setting.DepositAmount;
                        var origPoint = Math.Floor(sumRemainAmount / settingAmount);
                        
                        if (origPoint > 0)
                        {
                            var updateTime = custQueue.Max(x => x.DepositUpdateTime);
                            var pointRate = GetPointRate(setting, updateTime);
                            var mergeTransList = new List<TransInfo>();
                            
                            var usedAmount = origPoint * settingAmount;

                            foreach (var q in custQueue.OrderBy(x=>x.TransId))
                            {
                                if (usedAmount - q.RemainAmount >= 0)
                                {
                                    mergeTransList.Add(new TransInfo(q.TransId, q.RemainAmount));
                                    depositQueueInfos.Add(new DepositRewardPointQueueInfo
                                    {
                                        Status = (int) DepositRewardPointQueueStatusEnum.Finish,
                                        RemainAmount = 0,
                                        TransId = q.TransId
                                    });
                                }
                                else
                                {
                                    mergeTransList.Add(new TransInfo(q.TransId, usedAmount));
                                    depositQueueInfos.Add(new DepositRewardPointQueueInfo
                                    {
                                        Status = (int) DepositRewardPointQueueStatusEnum.Running,
                                        RemainAmount = q.RemainAmount - usedAmount,
                                        TransId = q.TransId
                                    });
                                    
                                    break;
                                }

                                usedAmount -= q.RemainAmount;
                            }

                            var transInfos = mergeTransList.Select(x => $"{x.TransId}: {x.Amount:G29}");
                            var memo = (mergeTransList.Count > 1
                                ? $"merged by {string.Join(", ", transInfos)} = {mergeTransList.Sum(x => x.Amount):G29}"
                                : transInfos.First()) + $", deposit amount {settingAmount:G29} per point, point rate: {pointRate:G29}";
                            
                            depositPointInfos.Add(new DepositRewardPointInfo
                            {
                                Memo = memo,
                                Point = origPoint * pointRate,
                                CustId = custQueue.Key,
                                SiteId = siteId,
                                TransId = mergeTransList.Last().TransId
                            });
                        }
                    }

                    // STEP4: 批次發放
                    if (depositQueueInfos.Any())
                    {
                        var input = new DepositBatchRewardPointParams
                        {
                            CurrencyId = group.Key,
                            DepositQueueInfos = depositQueueInfos,
                            DepositRewardPointInfos = depositPointInfos
                        };
                        
                        _logger.LogInformation("Process Queue: " + JsonConvert.SerializeObject(input));
                        
                        var success = await _rewardPointbll.DepositBatchRewardPointAsync(input);
                        if (!success)
                        {
                            var failedIds = input.DepositQueueInfos.Select(x => x.TransId);
                            _logger.LogError($"Deposit reward point failed, TransIds: {string.Join(',', failedIds)}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var failedIds = group.Select(x => x.TransId);
                    _logger.LogError(
                        $"Deposit reward point failed, Currency: {group.Key}, TransIds: {string.Join(',', failedIds)}", ex);
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

    private decimal GetPointRate(DepositPointSetting setting, DateTimeOffset updateTime)
    {
        if (!setting.PromotionStatus)
            return setting.PointRate;
        
        return updateTime >= setting.PromotionStartTime && updateTime < setting.PromotionEndTime
            ? setting.PromotionPointRate
            : setting.PointRate;
    }
}