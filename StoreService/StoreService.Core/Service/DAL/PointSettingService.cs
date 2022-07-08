using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.PointSetting;

namespace StoreService.Core.Service.DAL;

public class PointSettingService
{
    private readonly IConfiguration _configuration;
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;
    private readonly string _connectionString;

    public PointSettingService(IServiceProvider provider)
    {
        _configuration = provider.GetRequiredService<IConfiguration>();
        _connectionString = _configuration.GetConnectionString("ConnectionString");
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    public Task<IEnumerable<InactivePointExpirySetting>> GetInactivePointExpirySettingAsync(GetInactivePointExpirySettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);

        return _reportDbClient.QueryAsync<InactivePointExpirySetting>("Store_InactivePointExpiry_Get", para);
    }
    
    public Task<int> SetInactivePointExpirySettingAsync(SetInactivePointExpirySettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_Days", input.Days);
        para.Add("_Status", input.Status);
        para.Add("_Operator", input.Operator);

        return _mainDbClient.ExecuteAsync("Store_InactivePointExpiry_Set", para);
    }

    public Task<IEnumerable<TurnoverPointSetting>> GetTurnoverPointSettingAsync(GetTurnoverPointSettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_VipLevel", input.VipLevel);

        return _reportDbClient.QueryAsync<TurnoverPointSetting>("Store_TurnoverPointSetting_Get", para);
    }
    
    public Task<IEnumerable<SimpleTurnoverPointSetting>> GetTurnoverPointSettingListAsync(GetTurnoverPointSettingListParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);

        return _reportDbClient.QueryAsync<SimpleTurnoverPointSetting>("Store_TurnoverPointSettingList_Get", para);
    }
    
    public Task<int> SetTurnoverPointSettingAsync(SetTurnoverPointSettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Infos", input.StrInfos);
        para.Add("_Operator", input.Operator);

        return _mainDbClient.ExecuteAsync("Store_TurnoverPointSetting_Set", para);
    }
    
    public Task<IEnumerable<DepositPointSetting>> GetDepositPointSettingAsync(GetDepositPointSettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);

        return _reportDbClient.QueryAsync<DepositPointSetting>("Store_DepositPointSetting_Get", para);
    }
    
    public Task<int> SetDepositPointSettingAsync(SetDepositPointSettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_DepositAmount", input.DepositAmount);
        para.Add("_PointRate", input.PointRate);
        para.Add("_PromotionStatus", input.PromotionStatus);
        para.Add("_PromotionPointRate", input.PromotionPointRate);
        para.Add("_PromotionStartTime", input.PromotionStartTime);
        para.Add("_PromotionEndTime", input.PromotionEndTime);
        para.Add("_Operator", input.Operator);

        return _mainDbClient.ExecuteAsync("Store_DepositPointSetting_Set", para);
    }
    
    public Task<IEnumerable<TurnoverPointStatusSetting>> GetTurnoverPointStatusSettingAsync(GetTurnoverPointStatusSettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_TpCategoryId", input.TpCategoryId);

        return _reportDbClient.QueryAsync<TurnoverPointStatusSetting>("Store_TurnoverPointStatusSetting_Get", para);
    }
    
    public Task<int> SetTurnoverPointStatusSettingAsync(SetTurnoverPointStatusSettingParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Infos", input.StrInfos);
        para.Add("_Operator", input.Operator);

        return _mainDbClient.ExecuteAsync("Store_TurnoverPointStatusSetting_Set", para);
    }

    public Task<IEnumerable<TurnoverSettingForWorkerUse>> GetTurnoverSettingAsync(int siteId,int currencyId,string timezone)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", siteId);
        para.Add("_CurrencyId", currencyId);
        para.Add("_CurrencyTimeZone", timezone);

        return _reportDbClient.QueryAsync<TurnoverSettingForWorkerUse>("Service_StoreTurnoverSetting_Get", para);
    }
}