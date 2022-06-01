using Dapper;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.BaseData;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.PointSetting;

namespace StoreService.Core.Service.DAL;

public class CommonService
{
    private readonly MySqlClient _reportDbClient;

    public CommonService(IServiceProvider provider)
    {
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    public Task<IEnumerable<TpInfo>> GetTpListAsync(int siteId)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", siteId);
        
        return _reportDbClient.QueryAsync<TpInfo>("Store_TpList_Get", para);
    }
    
    public Task<IEnumerable<TpGameTypeInfo>> GetTpGameTypeListAsync(int siteId)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", siteId);
        
        return _reportDbClient.QueryAsync<TpGameTypeInfo>("Store_TpGameType_Get", para);
    }
    
    public Task<IEnumerable<VipLevel>> GetVipLevelListAsync(GetVipLevelListParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        
        return _reportDbClient.QueryAsync<VipLevel>("Store_VipLevel_Get", para);
    }
}