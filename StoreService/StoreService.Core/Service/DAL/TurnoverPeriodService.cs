using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.TurnoverPeriod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Core.Service.DAL;

public class TurnoverPeriodService
{
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;

    public TurnoverPeriodService(IServiceProvider provider)
    {
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    public async Task<IEnumerable<TurnoverPeriodForWorkerUse>> GetTurnoverPeriodParamAsync(bool isAll, int siteId)
    {
        var para = new DynamicParameters(new { _IsAll = isAll, _SiteId = siteId });
        return await _reportDbClient.QueryAsync<TurnoverPeriodForWorkerUse>("Service_TurnoverPeriod_Get", para);
    }

    public async Task<bool> SetTurnoverPeriodParamAsync(int siteId, int currencyId)
    {
        var para = new DynamicParameters(new { _SiteId = siteId, _CurrencyId = currencyId });
        return await _mainDbClient.ExecuteAsync("Service_TurnoverPeriod_DoneTask_Set", para) > 0;
    }
}
