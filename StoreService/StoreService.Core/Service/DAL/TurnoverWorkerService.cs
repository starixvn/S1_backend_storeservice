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

    public async Task<IEnumerable<TurnoverCustListModel>> GetCustListAsync(int siteId, int currencyId, string timezone, int strartIndex, int pageSize)
    {
        var para = new DynamicParameters(new { _SiteId = siteId, _CurrencyId = currencyId, _CurrencyTimeZone = timezone, _StartIndex = strartIndex, _PageSize = pageSize });
        return await _reportDbClient.QueryAsync<TurnoverCustListModel>("Service_StoreTurnoverCustList_Get", para);
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

    public async Task<bool> RemainRecordUpdateAsync(int custId, int categoryId, decimal remainAmount, MySqlConnection conn, MySqlTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_CustId", custId);
        para.Add("_CurrencyId", categoryId);
        para.Add("_ChangPoint", remainAmount);
        //para.Add("_BeforeRemainAmount", dbType: DbType.String, direction: ParameterDirection.Output);
        //para.Add("_AfterRemainAmount", dbType: DbType.String, direction: ParameterDirection.Output);
        //para.Add("_Result", dbType: DbType.String, direction: ParameterDirection.Output);

        //await conn.ExecuteAsync("Store_TurnOverRewardPointRemainRecord_Set", para, trans, commandType: CommandType.StoredProcedure);

        //var result = para.Get<decimal>("_Result");
        //var beforeBalance = para.Get<decimal>("_BeforeRemainAmount");
        //var afterBalance = para.Get<decimal>("_AfterRemainAmount");
        if (await conn.ExecuteAsync("Store_TurnOverRewardPointRemainRecord_Set", para, trans, commandType: CommandType.StoredProcedure) > 0)
        {
            return true;
        }
        return false;
    }
}
