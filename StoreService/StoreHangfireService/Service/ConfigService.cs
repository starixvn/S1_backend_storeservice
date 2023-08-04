using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;

namespace StoreHangfireService.Service;

public class ConfigService
{
    private readonly MySqlClient _reportDbClient;

    public ConfigService(IServiceProvider provider)
    {
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    public IEnumerable<T> Query<T>(string sql, object parameters = null)
    {
        return _reportDbClient.Query<T>(sql, parameters);
    }
}
