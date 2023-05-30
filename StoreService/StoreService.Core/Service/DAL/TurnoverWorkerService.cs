using Dapper;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.TurnoverWorker;
using System.Data;

namespace StoreService.Core.Service.DAL;

public class TurnoverWorkerService
{
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;

    public TurnoverWorkerService(IServiceProvider provider)
    {
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    public Task<IEnumerable<TurnoverSettingForWorkerUse>> GetTurnoverSettingAsync(int siteId, int currencyId, string timezone)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", siteId);
        para.Add("_CurrencyId", currencyId);
        para.Add("_CurrencyTimeZone", timezone);

        return _reportDbClient.QueryAsync<TurnoverSettingForWorkerUse>("Service_StoreTurnoverSetting_Get", para);
    }

    public async Task<IEnumerable<TurnoverCustListModel>> GetCustListAsync(int siteId, int currencyId, string timezone)
    {
        var para = new DynamicParameters(new { _SiteId = siteId, _CurrencyId = currencyId, _CurrencyTimeZone = timezone });
        return await _reportDbClient.QueryAsync<TurnoverCustListModel>("Service_StoreTurnoverCustList_Get_V3", para);
    }

    public async Task<IEnumerable<CustTurnoverBaseModel>> GetCustTurnoverAsync(int custId, string timezone)
    {
        var para = new DynamicParameters(new { _CustId = custId, _CurrencyTimeZone = timezone });
        return await _reportDbClient.QueryAsync<CustTurnoverBaseModel>("Service_StoreTurnoverTpBetRecord_Get", para);
    }

    public async Task<IEnumerable<CustRemainRecordModel>> GetRemainListAsync(int custId)
    {
        var para = new DynamicParameters(new { _CustId = custId });
        return await _reportDbClient.QueryAsync<CustRemainRecordModel>("Store_TurnOverRewardPointRemainRecord_Get", para);
    }

    public async Task<bool> RewardCategoryRecordCreateAsync(int custId, int categoryId, string workerDate, decimal addPoint, IDbTransaction trans)
    {
        var logPara = new DynamicParameters();
        logPara.Add("_CustId", custId);
        logPara.Add("_CategoryId", categoryId);
        logPara.Add("_WorkerDate", workerDate);
        logPara.Add("_PointEarned", addPoint);

        return await _mainDbClient.ExecuteAsync("Store_TurnOverRewardCategoryRecord_Ins", trans , logPara) > 0;
    }

    public async Task<bool> RemainRecordUpdateAsync(int custId, int categoryId, string workerDate, decimal remainAmount, IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_CustId", custId);
        para.Add("_CategoryId", categoryId);
        para.Add("_RemainAmount", remainAmount);
        para.Add("_BeforeRemain", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_AfterRemain", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _mainDbClient.ExecuteAsync("Store_TurnOverRewardPointRemainRecord_Set", trans ,para);

        var result = para.Get<int?>("_Result") ?? -9;
        var beforeRemain = para.Get<decimal>("_BeforeRemain");
        var afterRemain = para.Get<decimal>("_AfterRemain");

        if (result == 0)
        {
            var logPara = new DynamicParameters();
            logPara.Add("_CustId", custId);
            logPara.Add("_CategoryId", categoryId);
            logPara.Add("_WorkerDate", workerDate);
            logPara.Add("_ChangeRemain", remainAmount);
            logPara.Add("_BeforeRemain", beforeRemain);
            logPara.Add("_AfterRemain", afterRemain);

            var logResult = await _mainDbClient.ExecuteAsync("Store_TurnOverRemainRecordLog_Ins", trans , logPara);

            if (logResult > 0)
            {
                return true;
            }
        }

        return false;
    }
}
