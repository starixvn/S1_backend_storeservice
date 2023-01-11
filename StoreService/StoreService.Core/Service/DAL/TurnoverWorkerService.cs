using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.TurnoverWorker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        return await _reportDbClient.QueryAsync<TurnoverCustListModel>("Service_StoreTurnoverCustList_Get_V2", para);
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

    public async Task<bool> RewardCategoryRecordCreateAsync(int custId, int categoryId, string workerDate, decimal addPoint, MySqlConnection conn, MySqlTransaction trans)
    {
        var logPara = new DynamicParameters();
        logPara.Add("_CustId", custId);
        logPara.Add("_CategoryId", categoryId);
        logPara.Add("_WorkerDate", workerDate);
        logPara.Add("_PointEarned", addPoint);

        return await conn.ExecuteAsync("Store_TurnOverRewardCategoryRecord_Ins", logPara, trans, commandType: CommandType.StoredProcedure) > 0;
    }

    public async Task<bool> RemainRecordUpdateAsync(int custId, int categoryId, string workerDate, decimal remainAmount, MySqlConnection conn, MySqlTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_CustId", custId);
        para.Add("_CategoryId", categoryId);
        para.Add("_RemainAmount", remainAmount);
        para.Add("_BeforeRemain", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_AfterRemain", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await conn.ExecuteAsync("Store_TurnOverRewardPointRemainRecord_Set", para, trans, commandType: CommandType.StoredProcedure);

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

            var logResult = await conn.ExecuteAsync("Store_TurnOverRemainRecordLog_Ins", logPara, trans, commandType: CommandType.StoredProcedure);

            if (logResult > 0)
            {
                return true;
            }
        }

        return false;
    }
}
