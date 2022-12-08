using Newtonsoft.Json;
using StoreHangfireService.Job;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;
using StoreService.Core.Service.Model.PointSetting;
using StoreService.Core.Service.Model;
using StoreService.Core.Exceptions;
using StoreService.Core.Service.Model.RewardPoint;
using StoreService.Core.Service.DAL;

namespace StoreHangfireService.Service.Reward;

public class DepositRewardService
{
    private readonly RewardPointBLL _rewardPointbll;
    private readonly ILogger<BaseJob> _logger;

    public DepositRewardService(IServiceProvider provider)
    {
        _rewardPointbll = provider.GetRequiredService<RewardPointBLL>();
        _logger = provider.GetRequiredService<ILogger<BaseJob>>();
    }

    public async Task RunJobAsync()
    {
        // STEP1: 取得待處理佇列
        var queue = await GetDepositRewardPointQueue();

        if (!queue.Any())
        {
            _logger.LogWarning("No any data in the queue.");
            return;
        }

        // STEP2: 取得存款點數設定
        var settings = await GetAllDepositPointSettingAsync();

        if (!settings.Any())
        {
            _logger.LogWarning("GetAllDepositPointSetting Failed.");
            return;
        }

        // STEP3: 計算 & 發放點數
        await RewardPointAsync(queue, settings);

        await Task.CompletedTask;
    }

    private async Task<IEnumerable<DepositRewardPointQueue>> GetDepositRewardPointQueue()
    => await _rewardPointbll.GetDepositRewardPointQueueAsync();

    private async Task<IEnumerable<DepositPointSetting>> GetAllDepositPointSettingAsync()
        => await _rewardPointbll.GetAllDepositPointSettingAsync();

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
                                Status = (int)DepositRewardPointQueueStatusEnum.Finish,
                                RemainAmount = 0,
                                TransId = q.TransId
                            });
                        }
                        else
                        {
                            mergeTransList.Add(new TransInfo(q.TransId, usedAmount));
                            depositQueueInfos.Add(new DepositRewardPointQueueInfo
                            {
                                Status = (int)DepositRewardPointQueueStatusEnum.Running,
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
                    var input = new DepositBatchRewardPointParams
                    {
                        CurrencyId = group.Key,
                        DepositQueueInfos = depositQueueInfos,
                        DepositRewardPointInfos = depositPointInfos
                    };
                    _logger.LogWarning("Process Queue: " + JsonConvert.SerializeObject(input));

                    await BatchRewardPointAsync(input);
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
    private static decimal GetPointRate(DepositPointSetting setting, DateTimeOffset updateTime)
    {
        if (setting.PromotionStatus != GeneralStatusEnum.Enabled)
            return setting.PointRate;

        return updateTime >= setting.PromotionStartTime && updateTime < setting.PromotionEndTime
            ? setting.PromotionPointRate
            : setting.PointRate;
    }

    private async Task BatchRewardPointAsync(DepositBatchRewardPointParams input)
    {
        var success = await _rewardPointbll.DepositBatchRewardPointAsync(input);
        if (!success)
        {
            var failedIds = input.DepositQueueInfos.Select(x => x.TransId);
            _logger.LogError(
                $"Deposit reward point failed, TransIds: {string.Join(',', failedIds)}");
        }
    }
}
