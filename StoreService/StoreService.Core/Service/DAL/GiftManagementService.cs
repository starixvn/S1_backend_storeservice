using System.Data;

using Dapper;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Frontend;
using StoreService.Core.Service.Model.GiftManagement;

namespace StoreService.Core.Service.DAL;

public class GiftManagementService
{
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;


    public GiftManagementService(IServiceProvider provider)
    {
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Report);
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

        para.Add("_Langs", input.StrLangCodes);

        return _reportDbClient.QueryAsync<GiftTag>("Store_GiftTag_Get", para);
    }

    public Task<IEnumerable<GiftCategory>> GetCategoriesAsync(GetCategoriesParams input)
    {
        var para = new DynamicParameters();

        para.Add("_Langs", input.StrLangCodes);

        return _reportDbClient.QueryAsync<GiftCategory>("Store_GiftCategory_Get", para);
    }

    public Task<IEnumerable<GiftInfo>> GetGiftAsync(GetGiftParams input)
    {
        var para = new DynamicParameters();

        para.Add("_GiftId", input.GiftId);

        return _reportDbClient.QueryAsync<GiftInfo>("Store_GiftInfo_Get_V2", para);
    }

    public async Task<int> CreateGiftAsync(CreateGiftParams input)
    {
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
        para.Add("_GiftType", input.GiftType);
        para.Add("_Round", input.Round);
        para.Add("_Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();

        _ = _mainDbClient.ExecuteAsync("Store_GiftInfo_Ins_V2", trans, para);
        var resultCode = para.Get<int?>("_Result") ?? -9;

        if (resultCode == 0)
        {
            trans.Commit();
        }
        else
        {
            trans.Rollback();
        }

        await conn.CloseAsync();
        await conn.DisposeAsync();

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
        para.Add("_UseCategoryIds", input.UseCategoryIds);
        para.Add("_CategoryIds", input.StrCategoryIds);
        para.Add("_UseTags", input.UseTags);
        para.Add("_Tags", input.StrTags);
        para.Add("_Lang", input.Lang);
        para.Add("_OrderByField", input.OrderByField);
        para.Add("_Desc", input.Desc);
        para.Add("_PageSize", input.PageSize);

        return _reportDbClient.QueryAsync<GiftListModel>("Store_Frontend_GiftList_Get_V2", para);
    }

    public Task<IEnumerable<Gift>> GetShoppingCartAsync(SetShoppingCartRequest request)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", request.SiteId);
        para.Add("_CurrencyId", request.CurrencyId);
        para.Add("_CustId", request.CustId);
        return _mainDbClient.QueryAsync<Gift>("Store_CustShoppingCart_Get", para);
    }

    public async Task<int> SetShoppingCartAsync(SetShoppingCartRequest input)
    {
        var para = new DynamicParameters();
        para.Add("_CustId", input.CustId);
        para.Add("_GiftIds", JsonConvert.SerializeObject(input.GiftIds));
        para.Add("_IsAdd", input.IsAdd);

        return await _mainDbClient.ExecuteAsync("Store_CustShoppingCart_Set", para);
    }
}
