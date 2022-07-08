using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
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
    private readonly PointSettingService _pointSettingService;
    private readonly TurnoverWorkerService _turnoverWorkerService;
    private readonly RewardPointService _rewardPointService;
    private readonly ILogger<TurnoverWorkerBLL> _logger;
    private readonly MySqlClient _mainDbClient;

    public TurnoverWorkerBLL(IServiceProvider provider)
    {
        _turnoverPeriodService = provider.GetRequiredService<TurnoverPeriodService>();
        _pointSettingService = provider.GetRequiredService<PointSettingService>();
        _turnoverWorkerService = provider.GetRequiredService<TurnoverWorkerService>();
        _rewardPointService = provider.GetRequiredService<RewardPointService>();
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
        => _pointSettingService.GetTurnoverSettingAsync(siteId, currencyId, timezone);

    public Task<IEnumerable<TurnoverCustListModel>> GetCustListAsync(int siteId, int currencyId, string timezone, int pageNumber, int pageSize)
        => _turnoverWorkerService.GetCustListAsync(siteId, currencyId, timezone, Math.Abs(pageNumber - 1) * Math.Abs(pageSize), pageSize);

    public Task<IEnumerable<CustTurnoverBaseModel>> GetCustTurnoverAsync(int custId, string timezone)
        => _turnoverWorkerService.GetCustTurnoverAsync(custId, timezone);

    public Task<IEnumerable<CustRemainRecordModel>> GetCustRemainListAsync(int custId)
    => _turnoverWorkerService.GetRemainListAsync(custId);

    public List<CustAddPointModel> CaculatePoint(TurnoverCustListModel custData,
        IEnumerable<CustTurnoverBaseModel> turnoverData,
        IEnumerable<CustRemainRecordModel> remainData,
        IEnumerable<TurnoverSettingForWorkerUse> settingLists)
    {
        var result = new List<CustAddPointModel>();
        var settings = settingLists.Where(x => x.VipLevel == custData.VipLevel);

        var validTurnoverData = turnoverData.Where(x => settings.Select(x => x.TpGameTypeId).Contains(x.GameType))
            .GroupBy(g => new { g.CategoryId })
            .Select(x => new
            {
                CategoryId = x.Key.CategoryId,
                ValidBet = turnoverData.Where(n => n.CategoryId == x.Key.CategoryId).Sum(x => x.ValidBet),
            }).ToList();

        foreach (var data in validTurnoverData)
        {
            var setting = CheckRate(settings.FirstOrDefault(x => x.TpCategoryId == data.CategoryId));
            var remainRecord = remainData.FirstOrDefault(x => x.CategoryId == data.CategoryId);
            var caculateResult = Caculate((data.ValidBet + remainRecord.RemainAmount), setting.Turnover, setting.Rate);
            result.Add(new CustAddPointModel()
            {
                SettledDate = turnoverData.First().SettledDate.ToString("yyyyMMdd"),
                CategoryId = data.CategoryId,
                IsUsePromotionRate = setting.IsUsePromotionRate,
                Rate = setting.Rate,
                UsedTurnover = caculateResult.UseTurnover,
                UnUseTurnOver = caculateResult.UnUseTurnover,
                AddPoint = caculateResult.AddPoint
            });
        }
        return result;
    }

    public async Task<bool> RewardPointAsync(int siteId, int currencyId, int custId, List<CustAddPointModel> addPointData)
    {
        var memo = GenMemo(addPointData);

        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();

        //Update Point + Insert Log

        var result = await _rewardPointService.CustRewardPointChange(new CustRewardPointChangeParams()
        {
            SiteId = siteId,
            CurrencyId = currencyId,
            CustId = custId,
            pointType = PointTypeEnum.Turnover,
            ChangePoint = addPointData.Sum(x => x.AddPoint),
            TransId = addPointData.First().SettledDate,
            Memo = memo
        }, conn, trans);

        //Queue

        if (!result)
        {
            trans.Rollback();
            return false;
        }

        foreach (var data in addPointData)
        {
            if (!await _turnoverWorkerService.RemainRecordUpdateAsync(custId, data.CategoryId, data.UnUseTurnOver, conn, trans))
            {
                trans.Rollback();
                return false;
            }
        }

        trans.Commit();
        return true;
    }

    public Task<bool> UpdateTaskSettledTImeASync(int siteId, int currencyId)
    => _turnoverPeriodService.SetTurnoverPeriodParamAsync(siteId, currencyId);

    private Task<IEnumerable<TurnoverPeriodForWorkerUse>> GetTurnoverPeriodParamAsync()
        => _turnoverPeriodService.GetTurnoverPeriodParamAsync(true, 0);

    private (bool IsUsePromotionRate, decimal Turnover, decimal Rate) CheckRate(TurnoverSettingForWorkerUse setting)
    {
        var Turnover = setting.Turnover;
        var IsUsePromotionRate = setting.PromotionStatus && setting.IsInPromotionPeriod;
        var Rate = IsUsePromotionRate ? setting.PromotionPointRate : setting.PointRate;

        return (IsUsePromotionRate, Turnover, Rate);
    }

    private (decimal UseTurnover, decimal UnUseTurnover, decimal AddPoint) Caculate(decimal validBet, decimal turnover, decimal rate)
    {
        var UnUserTurnover = validBet % turnover;
        var AddPoint = validBet / turnover * rate;
        var UseTurnover = turnover * AddPoint;

        return (UseTurnover, UnUserTurnover, AddPoint);
    }

    private string GenMemo(List<CustAddPointModel> addPointData)
    {
        var sb = new StringBuilder();
        foreach(var data in addPointData)
        {
            
        }
        return sb.ToString();
    }

    //public class CustAddPointModel
    //{
    //    public string SettledDate { get; set; }
    //    public int CategoryId { get; set; }
    //    public bool IsUsePromotionRate { get; set; }
    //    public decimal Rate { get; set; }
    //    public decimal UsedTurnover { get; set; }
    //    public decimal UnUseTurnOver { get; set; }
    //    public decimal AddPoint { get; set; }
    //}
}
