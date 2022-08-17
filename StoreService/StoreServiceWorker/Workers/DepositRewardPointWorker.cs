using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.PointSetting;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreServiceWorker.Workers;

public class DepositRewardPointWorker : BackgroundService
{
    private readonly RewardPointBLL _rewardPointbll;
    private readonly ILogger<DepositRewardPointWorker> _logger;
    private readonly IWebHostEnvironment _env;
    private Timer _timer;

    public DepositRewardPointWorker(IServiceProvider provider)
    {
        _rewardPointbll = provider.GetRequiredService<RewardPointBLL>();
        _logger = provider.GetRequiredService<ILogger<DepositRewardPointWorker>>();
        _env = provider.GetRequiredService<IWebHostEnvironment>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);

        return Task.CompletedTask;
    }

    private async void DoTaskAsync(object state)
    {
        using (LogContext.PushProperty("WorkerName", "DepositRewardPointWorker"))
        using (LogContext.PushProperty("TaskId", Guid.NewGuid().ToString()))
        {
            try
            {
                // STEP1: 取得待處理佇列
                var queue = await _rewardPointbll.GetDepositRewardPointQueueAsync();

                if (!queue.Any())
                {
                    _logger.LogInformation("No any data in the queue.");
                    return;
                }

                // STEP2: 取得存款點數設定
                var settings = await _rewardPointbll.GetAllDepositPointSettingAsync();

                if (!settings.Any())
                {
                    _logger.LogError("GetAllDepositPointSetting Failed.");
                    return;
                }

                // STEP3: 計算 & 發放點數
                await RewardPointAsync(queue, settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DepositRewardPointWorker Exception");
            }
            finally
            {
                var envName = _env.EnvironmentName;
                var ts = envName is "Development" or "Local"
                    ? TimeSpan.FromMinutes(1)
                    : TimeSpan.FromMinutes(5);
                
                _timer.Change(ts, TimeSpan.Zero);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    #region Private Methods

    private async Task RewardPointAsync(IEnumerable<DepositRewardPointQueue> queue,
        IEnumerable<DepositPointSetting> settings)
    {
        var groupQueue = queue.GroupBy(x => x.CurrencyId);
        foreach (var group in groupQueue)
        {
            var currencyId = group.Key;
            var depositQueueInfos = new List<DepositRewardPointQueueInfo>();
            var depositPointInfos = new List<DepositRewardPointInfo>();

            try
            {
                var groupByCustQueue = group.GroupBy(x => x.CustId);

                // 依用戶計算點數
                foreach (var custQueue in groupByCustQueue)
                {
                    var siteId = custQueue.First().SiteId;
                    var setting = settings.First(x => x.SiteId == siteId && x.CurrencyId == currencyId);
                    var sumRemainAmount = custQueue.Sum(x => x.RemainAmount);
                    var settingAmount = setting.DepositAmount;
                    var origPoint = Math.Floor(sumRemainAmount / settingAmount);

                    if (origPoint <= 0)
                        continue;

                    var updateTime = custQueue.Max(x => x.DepositUpdateTime);
                    var pointRate = GetPointRate(setting, updateTime);
                    var mergeTransList = new List<TransInfo>();

                    var usedAmount = origPoint * settingAmount;

                    foreach (var q in custQueue.OrderBy(x => x.TransId))
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
                                   : transInfos.First()) +
                               $", deposit amount {settingAmount:G29} per point, point rate: {pointRate:G29}";

                    depositPointInfos.Add(new DepositRewardPointInfo
                    {
                        Memo = memo,
                        Point = origPoint * pointRate,
                        CustId = custQueue.Key,
                        SiteId = siteId,
                        TransId = mergeTransList.Last().TransId
                    });
                }

                // 批次發放
                if (depositQueueInfos.Any())
                {
                    await BatchRewardPointAsync(group.Key, depositQueueInfos, depositPointInfos);
                }
            }
            catch (Exception ex)
            {
                var failedIds = group.Select(x => x.TransId);
                _logger.LogError(
                    $"Deposit reward point failed, Currency: {group.Key}, TransIds: {string.Join(',', failedIds)}",
                    ex);
            }
        }
    }

    private async Task BatchRewardPointAsync(int currencyId,
        IEnumerable<DepositRewardPointQueueInfo> queueInfos,
        IEnumerable<DepositRewardPointInfo> rewardPointInfos)
    {
        var input = new DepositBatchRewardPointParams
        {
            CurrencyId = currencyId,
            DepositQueueInfos = queueInfos,
            DepositRewardPointInfos = rewardPointInfos
        };

        _logger.LogInformation("Process Queue: " + JsonConvert.SerializeObject(input));

        var success = await _rewardPointbll.DepositBatchRewardPointAsync(input);
        if (!success)
        {
            var failedIds = input.DepositQueueInfos.Select(x => x.TransId);
            _logger.LogError(
                $"Deposit reward point failed, TransIds: {string.Join(',', failedIds)}");
        }
    }

    private static decimal GetPointRate(DepositPointSetting setting, DateTimeOffset updateTime)
    {
        if (setting.PromotionStatus != GeneralStatusEnum.Enabled)
            return setting.PointRate;

        return updateTime >= setting.PromotionStartTime && updateTime < setting.PromotionEndTime
            ? setting.PromotionPointRate
            : setting.PointRate;
    }

    #endregion
}