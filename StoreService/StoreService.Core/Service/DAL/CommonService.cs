using Microsoft.Extensions.DependencyInjection;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.BaseData;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.DAL;

public class CommonService
{
    private readonly MySqlClient _reportDbClient;

    public CommonService(IServiceProvider provider)
    {
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }
    
    public Task<IEnumerable<TpCategory>> GetTpCategoryAsync()
        => _reportDbClient.QueryAsync<TpCategory>("Common_TpCategory_Get");
}