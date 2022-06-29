using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.GiftManagement;
using StoreService.Core.Service.Model.Frontend;
using StoreService.Core.Service.Model.RewardPoint;
using StoreService.Extensions;

namespace StoreService.Core.Service.DAL;

public class GiftManagementService
{
    private readonly IConfiguration _configuration;
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;
    private readonly RewardPointService _rewardPointService;
    
    private readonly string _connectionString;

    public GiftManagementService(IServiceProvider provider)
    {
        _configuration = provider.GetRequiredService<IConfiguration>();
        _connectionString = _configuration.GetConnectionString("ConnectionString");
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
        _rewardPointService = provider.GetRequiredService<RewardPointService>();
    }

    public Task<IEnumerable<Gift>> GetGiftListAsync(GetListParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Status", input.StrStatus);
        para.Add("_CategoryIds", input.StrCategoryIds);
        para.Add("_Tags", input.StrTags);
        para.Add("_GiftName", input.GiftName);
        para.Add("_OrderByField", input.OrderByField);
        para.Add("_Desc", input.Desc);

        return _reportDbClient.QueryAsync<Gift>("Store_GiftList_Get", para);
    }

    public Task<IEnumerable<GiftTag>> GetTagsAsync(GetTagsParams input)
    {
        var para = new DynamicParameters();

        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Langs", input.StrLangCodes);

        return _reportDbClient.QueryAsync<GiftTag>("Store_GiftTag_Get", para);
    }

    public Task<IEnumerable<GiftCategory>> GetCategoriesAsync(GetCategoriesParams input)
    {
        var para = new DynamicParameters();

        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Langs", input.StrLangCodes);

        return _reportDbClient.QueryAsync<GiftCategory>("Store_GiftCategory_Get", para);
    }

    public Task<IEnumerable<GiftInfo>> GetGiftAsync(GetGiftParams input)
    {
        var para = new DynamicParameters();

        para.Add("_GiftId", input.GiftId);

        return _reportDbClient.QueryAsync<GiftInfo>("Store_GiftInfo_Get", para);
    }

    public async Task<int> CreateGiftAsync(CreateGiftParams input)
    {
        var resultCode = -9;
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Status", input.Status);
        para.Add("_CategoryId", input.CategoryId);
        para.Add("_Tags", input.StrTags);
        para.Add("_Point", input.Point);
        para.Add("_PriceWorth", input.PriceWorth);
        para.Add("_ShowStock", input.ShowStock);
        para.Add("_Stock", input.Stock);
        para.Add("_Infos", input.StrInfos);
        para.Add("_Operator", input.Operator);
        para.Add("_Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();

            await using (var trans = conn.BeginTransaction())
            {
                _ = conn.ExecuteAsync("Store_GiftInfo_Ins", para, transaction: trans,
                    commandType: CommandType.StoredProcedure);
                resultCode = para.Get<int?>("_Result") ?? -9;

                if (resultCode == 0)
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                }
            }

            await conn.CloseAsync();
        }

        return resultCode;
    }

    public async Task<bool> UpdateGiftAsync(UpdateGiftParams input)
    {
        var para = new DynamicParameters();
        para.Add("_GiftId", input.GiftId);
        para.Add("_Status", input.Status);
        para.Add("_CategoryId", input.CategoryId);
        para.Add("_Tags", input.StrTags);
        para.Add("_Point", input.Point);
        para.Add("_PriceWorth", input.PriceWorth);
        para.Add("_ShowStock", input.ShowStock);
        para.Add("_Stock", input.Stock);
        para.Add("_Infos", input.StrInfos);
        para.Add("_Operator", input.Operator);

        _ = await _mainDbClient.ExecuteAsync("Store_GiftInfo_Set", para);

        return true;
    }

    public Task<int> CreateCategoryAsync(CreateCategoryParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Infos", input.StrInfos);

        var result = _mainDbClient.ExecuteAsync("Store_GiftCategory_Ins", para);

        return result;
    }

    public Task<int> UpdateCategoryAsync(UpdateCategoryParams input)
    {
        var para = new DynamicParameters();
        para.Add("_CategoryId", input.CategoryId);
        para.Add("_Infos", input.StrInfos);

        var result = _mainDbClient.ExecuteAsync("Store_GiftCategory_Set", para);

        return result;
    }

    public Task<int> CreateTagAsync(CreateTagParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Infos", input.StrInfos);

        var result = _mainDbClient.ExecuteAsync("Store_GiftTag_Ins", para);

        return result;
    }

    public Task<int> UpdateTagAsync(UpdateTagParams input)
    {
        var para = new DynamicParameters();
        para.Add("_TagId", input.TagId);
        para.Add("_Infos", input.StrInfos);

        var result = _mainDbClient.ExecuteAsync("Store_GiftTag_Set", para);

        return result;
    }
    
    public async Task<bool> CreateRedemptionAsync(CreateRedemptionParams input )
    {
        var para = new DynamicParameters();
        para.Add("_GiftId", input.GiftId);
        para.Add("_CustId", input.CustId);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Point", input.Point);
        para.Add("_Prefix", "RS");
        para.Add("_TransId", dbType: DbType.String, direction: ParameterDirection.Output);

        await using var conn = new MySqlConnection(_connectionString);
        conn.Open();

        await using (var trans = conn.BeginTransaction())
        {
            _ = conn.ExecuteAsync("Store_RedemptionInfo_Set", para, trans, commandType: CommandType.StoredProcedure);
            var transId = para.Get<string>("_TransId") ?? string.Empty;

            if (!string.IsNullOrEmpty(transId))
            {
                var custRewardPointUseRequest = new CustRewardPointUseParams
                {
                    CustId = input.CustId,
                    CurrencyId = input.CurrencyId,
                    SiteId = input.SiteId,
                    UsePoint = input.Point
                };
                var pointUseResult = await _rewardPointService.CustRewardPointUse(custRewardPointUseRequest,transId,conn,trans);
                    
                if (pointUseResult)
                {
                    trans.Commit();
                    return pointUseResult;
                }

                trans.Rollback();
            }
            else
            {
                trans.Rollback();
            }
        }

        await conn.CloseAsync();

        return false;
    }
    
    public async Task<RedemptionLimit> GetRedemptionLimitAsync(GetRedemptionLimitParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);

        var result =
                await _mainDbClient.QueryAsync<RedemptionLimit>("Store_RedemptionLimit_Get", para);

        return result.FirstOrDefault();
    }

    public Task<int> SetRedemptionLimitAsync(SetRedemptionLimitParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_Limit", input.Limit);
        para.Add("_Status", input.Status);
        para.Add("_Operator", input.Operator);

        var result = _mainDbClient.ExecuteAsync("Store_RedemptionLimit_Set", para);

        return result;
    }

    public Task<int> SetGiftSortAsync(SetSortParams input)
    {
        var para = new DynamicParameters();
        para.Add("_Infos", input.StrInfos);
        para.Add("_Operator", input.Operator);

        var result = _mainDbClient.ExecuteAsync("Store_GiftSort_Set", para);

        return result;
    }

    public Task<IEnumerable<GiftListModel>> GetGiftListAsync(FrontendGetListParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_CategoryIds", input.StrCategoryIds);
        para.Add("_Tags", input.StrTags);
        para.Add("_Lang", input.Lang);
        para.Add("_OrderByField", input.OrderByField);
        para.Add("_Desc", input.Desc);

        return _reportDbClient.QueryAsync<GiftListModel>("Store_Frontend_GiftList_Get", para);
    }
}
