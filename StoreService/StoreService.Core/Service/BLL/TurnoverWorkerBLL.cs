using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.PointSetting;
using StoreService.Core.Service.Model.RewardPoint;
using StoreService.Core.Service.Model.TurnoverWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Core.Service.BLL;

public class TurnoverWorkerBLL
{

    private readonly TurnoverPeriodService _turnoverPeriodService;
    private readonly TurnoverWorkerService _turnoverWorkerService;
    private readonly RewardPointService _rewardPointService;
    private readonly CommonService _commonService;
    private readonly ILogger<TurnoverWorkerBLL> _logger;
    private readonly MySqlClient _mainDbClient;

    public TurnoverWorkerBLL(IServiceProvider provider)
    {
        _turnoverPeriodService = provider.GetRequiredService<TurnoverPeriodService>();
        _turnoverWorkerService = provider.GetRequiredService<TurnoverWorkerService>();
        _rewardPointService = provider.GetRequiredService<RewardPointService>();
        _commonService = provider.GetRequiredService<CommonService>();
        _logger = provider.GetRequiredService<ILogger<TurnoverWorkerBLL>>();
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
    }

    public async Task<IEnumerable<TurnoverPeriodForWorkerUse>> VerifyTaskAsync()
    {
        var rebateTaskList = await GetTurnoverPeriodParamAsync();

        var result = new List<TurnoverPeriodForWorkerUse>();
        foreach (var task in rebateTaskList)
        {

            //檢查小時是否符合
            //幣別時區小時 <= 指定小時，不執行排程
            if (task.CurrencyNowTime.Hour <= task.RedeemHours)
            {
                _logger.LogWarning($"task.RedeemsHours Invaild. task value={JsonConvert.SerializeObject(task)}");
                continue;
            }

            //檢查最近一次執行時間是否符合
            //CurrencyLastSettledTime.Date = CurrencyNowTime.Date，上次執行時間日期等於幣別時區時間日期，不可再執行(原邏輯)
            if (task.CurrencyLastSettledTime.Date == task.CurrencyNowTime.Date)
            {
                _logger.LogWarning($"task.LastCurrencySettledTime Invaild. task value={JsonConvert.SerializeObject(task)}");
                continue;
            }
            result.Add(task);
        }

        return result;
    }

    public Task<IEnumerable<TurnoverSettingForWorkerUse>> GetSettingAsync(int siteId, int currencyId, string timezone)
        => _turnoverWorkerService.GetTurnoverSettingAsync(siteId, currencyId, timezone);

    public Task<IEnumerable<TurnoverCustListModel>> GetCustListAsync(int siteId, int currencyId, string timezone, int pageNumber, int pageSize)
        => _turnoverWorkerService.GetCustListAsync(siteId, currencyId, timezone, Math.Abs(pageNumber - 1) * Math.Abs(pageSize), pageSize);

    public async Task<bool> CaculatePointAsync(TurnoverPeriodForWorkerUse task,
        IEnumerable<TurnoverCustListModel> custList,
        IEnumerable<TurnoverSettingForWorkerUse> pointSetting)
    {
        if (!pointSetting.Any())
        {
            _logger.LogWarning($"TurnoverWorker：Not Exist Turnover Setting. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.");
            return true;
        }
        var settleDate = custList.First().CaculateDate;
        var LevelList = custList.Select(x => x.VipLevel).Distinct();
        var tpCategoryInfoList = await GetTpCategoryInfoAsync();
        foreach (var level in LevelList)
        {
            var settings = pointSetting.Where(x => x.VipLevel == level);
            var levelCustList = custList.Where(x => x.VipLevel == level);

            foreach (var custData in levelCustList)
            {
                var turnoverData = await GetCustTurnoverAsync(custData.CustId, task.TimeZone);

                var remainData = await GetCustRemainListAsync(custData.CustId);

                var caculateData = Caculate(turnoverData, remainData, settings);

                if (caculateData.Count > 0 && !await RewardPointAsync(task.SiteId, task.CurrencyId, custData.CustId, settleDate, caculateData, tpCategoryInfoList))
                {
                    _logger.LogError($"TurnoverWorker：Cust Reward Point Fail. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId} CustId:{custData.CustId}.");
                    return false;
                }
            }
        }
        return true;
    }


    public List<CustAddPointModel> Caculate(IEnumerable<CustTurnoverBaseModel> turnoverData,
        IEnumerable<CustRemainRecordModel> remainData,
        IEnumerable<TurnoverSettingForWorkerUse> settingLists)
    {
        var result = new List<CustAddPointModel>();

        var validTurnoverData = turnoverData.Where(x => settingLists.Select(x => x.TpGameTypeId).Contains(x.GameType))
            .GroupBy(g => new { g.CategoryId })
            .Select(x => new
            {
                CategoryId = x.Key.CategoryId,
                ValidBet = turnoverData.Where(n => n.CategoryId == x.Key.CategoryId).Sum(x => x.ValidBet),
            }).ToList();

        foreach (var data in validTurnoverData)
        {
            var setting = CheckRate(settingLists.FirstOrDefault(x => x.TpCategoryId == data.CategoryId));
            var remainAmount = remainData.FirstOrDefault(x => x.CategoryId == data.CategoryId)?.RemainAmount ?? 0;
            var caculateResult = Caculate(data.ValidBet, remainAmount, setting.Turnover, setting.Rate);
            result.Add(new CustAddPointModel()
            {
                SettledDate = turnoverData.First().SettledDate.ToString("yyyyMMdd"),
                CategoryId = data.CategoryId,
                IsUsePromotionRate = setting.IsUsePromotionRate,
                RateTurnover = setting.Turnover,
                Rate = setting.Rate,
                AddPoint = caculateResult.TotalAddPoint,
                ThisTurnover = data.ValidBet,
                ThisRemain = remainAmount,
                UsedTurnover = caculateResult.UsedTurnover,
                UnUseTurnover = caculateResult.UnUseTurnover,
                UsedRemain = caculateResult.UsedRemain,
                UnUseRemain = caculateResult.UnUseRemain,
                ChangeRemain = caculateResult.ChangeRemain
            });
        }
        return result;
    }

    public Task<bool> UpdateTaskSettledTImeASync(int siteId, int currencyId)
    => _turnoverPeriodService.SetTurnoverPeriodParamAsync(siteId, currencyId);

    #region Private Methods

    private Task<IEnumerable<TurnoverPeriodForWorkerUse>> GetTurnoverPeriodParamAsync()
        => _turnoverPeriodService.GetTurnoverPeriodParamAsync(true, 0);

    private Task<IEnumerable<CustTurnoverBaseModel>> GetCustTurnoverAsync(int custId, string timezone)
    => _turnoverWorkerService.GetCustTurnoverAsync(custId, timezone);

    private Task<IEnumerable<CustRemainRecordModel>> GetCustRemainListAsync(int custId)
    => _turnoverWorkerService.GetRemainListAsync(custId);

    private async Task<bool> RewardPointAsync(int siteId, int currencyId, int custId, string settleDate,
        List<CustAddPointModel> addPointData,
        IEnumerable<TpCategoryInfo> tpCategoryInfoList)
    {
        var memo = GenMemo(addPointData, tpCategoryInfoList);

        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();

        if (addPointData.Sum(x => x.AddPoint) > 0)
        {
            //Update Point + Insert Log
            var result = await _rewardPointService.CustRewardPointChange(new CustRewardPointChangeParams()
            {
                SiteId = siteId,
                CurrencyId = currencyId,
                CustId = custId,
                pointType = PointTypeEnum.Turnover,
                ChangePoint = addPointData.Sum(x => x.AddPoint),
                TransId = settleDate,
                Memo = memo
            }, conn, trans);

            if (!result)
            {
                trans.Rollback();
                return false;
            }
        }

        //Update Remain
        foreach (var data in addPointData.Where(x => x.ChangeRemain != 0))
        {
            if (!await _turnoverWorkerService.RemainRecordUpdateAsync(custId, data.CategoryId, data.ChangeRemain, settleDate, conn, trans))
            {
                trans.Rollback();
                return false;
            }
        }

        trans.Commit();
        return true;
    }

    private (bool IsUsePromotionRate, decimal Turnover, decimal Rate) CheckRate(TurnoverSettingForWorkerUse setting)
    {
        var Turnover = setting.Turnover;
        var IsUsePromotionRate = setting.PromotionStatus && setting.IsInPromotionPeriod;
        var Rate = IsUsePromotionRate ? setting.PromotionPointRate : setting.PointRate;

        return (IsUsePromotionRate, Turnover, Rate);
    }

    private (decimal UsedTurnover, decimal UnUseTurnover, decimal UsedRemain, decimal UnUseRemain, decimal ChangeRemain, decimal TotalAddPoint) Caculate
        (decimal validBet, decimal remain, decimal rateTurnover, decimal rate)
    {
        if (rateTurnover <= 0 || rate <= 0)
            return (0, validBet, 0, remain, validBet, 0);

        var totalTurnover = validBet + remain;
        var totalUnUsedTurnover = totalTurnover % rateTurnover;
        var addPoint = Math.Truncate(totalTurnover / rateTurnover * rate);
        var totalUsedTurnover = rateTurnover * addPoint / rate;

        decimal usedTurnover = 0;
        decimal unUseTurnover = 0;
        decimal usedRemain = 0;
        decimal unUseRemain = 0;
        decimal changeRemain = 0;

        if (totalUsedTurnover > validBet)
        {
            //validbet全部使用+使用結餘
            usedTurnover = validBet;
            usedRemain = totalUsedTurnover - validBet;
            unUseRemain = totalUnUsedTurnover;
            changeRemain = -usedRemain;
        }
        else
        {
            //validbet沒有全部使用=>未使用與結餘合併
            usedTurnover = totalUsedTurnover;
            unUseTurnover = validBet - totalUsedTurnover;
            unUseRemain = remain + unUseTurnover;
            changeRemain = unUseTurnover;
        }
        return (usedTurnover, unUseTurnover, usedRemain, unUseRemain, changeRemain, addPoint);
    }

    private string GenMemo(List<CustAddPointModel> addPointData, IEnumerable<TpCategoryInfo> tpCategoryInfoList)
    {
        var sb = new StringBuilder();
        foreach (var data in addPointData)
        {
            sb.Append($"Category:{tpCategoryInfoList.First(x => x.CategoryId == data.CategoryId).CategoryName},");
            sb.Append($"(Turnover{data.ThisTurnover.ToString("0.####")}");
            if (data.ThisRemain > 0)
                sb.Append($"+Balance{data.ThisRemain.ToString("0.####")}");
            sb.Append(")");
            if (data.IsUsePromotionRate)
                sb.Append($"IsInPromotionPeriod*{data.Rate.ToString("0.####")}");

            sb.Append($" ( Balance{data.UnUseRemain.ToString("0.####")} ).\n");
        }
        return sb.ToString();
    }


    private async Task<IEnumerable<TpCategoryInfo>> GetTpCategoryInfoAsync()
    {
        return await _commonService.GetTpCategoryInfoAsync();
    }
    #endregion
}
