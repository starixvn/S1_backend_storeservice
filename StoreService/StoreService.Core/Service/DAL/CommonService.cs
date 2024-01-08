using Microsoft.Extensions.DependencyInjection;

using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.DAL;

public class CommonService
{
    private readonly MySqlClient _reportDbClient;

    public CommonService(IServiceProvider provider)
    {
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    public Task<IEnumerable<CurrencyInfo>> GetCurrencyInfoAsync()
    {
        return _reportDbClient.QueryAsync<CurrencyInfo>("Common_Currency_Get");
    }

    public Task<IEnumerable<TpCategoryInfo>> GetTpCategoryInfoAsync()
    {
        return _reportDbClient.QueryAsync<TpCategoryInfo>("Admin_TPCategory_Get");
    }
}