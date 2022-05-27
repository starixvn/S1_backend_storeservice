using Newtonsoft.Json;
using StoreService.Core.Extensions;
using StoreService.Core.Service.Model.Based;

namespace StoreService.Core.Service.Model.GiftManagement;

public class GetListParams : PagerParamModel
{
    public GetListParams(GetListRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        StrStatus = JsonConvert.SerializeObject(req.Status);
        StrCategoryIds = JsonConvert.SerializeObject(req.CategoryIds);
        StrTags = JsonConvert.SerializeObject(req.Tags);
        GiftName = req.GiftName;
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string StrStatus { get; set; }
    public string StrCategoryIds { get; set; }
    public string StrTags { get; set; }
    public string GiftName { get; set; }
}

public class GetTagsParams
{
    public GetTagsParams()
    {
    }
    
    public GetTagsParams(GetTagsRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        StrLangCodes = JsonConvert.SerializeObject(req.LangCodes);
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string StrLangCodes { get; set; }
}

public class GetCategoriesParams
{
    public GetCategoriesParams(GetCategoriesRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        StrLangCodes = JsonConvert.SerializeObject(req.LangCodes);
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string StrLangCodes { get; set; }
}

public class CreateGiftParams
{
    public CreateGiftParams(CreateGiftRequest req, string @operator)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        Status = (int) req.Status;
        CategoryId = req.CategoryId;
        StrTags = JsonConvert.SerializeObject(req.Tags);
        Point = req.Point;
        PriceWorth = req.PriceWorth;
        ShowStock = (int) req.ShowStock;
        Stock = req.Stock;
        Sort = req.Sort;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
        Operator = @operator;
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int Status { get; set; }
    public int CategoryId { get; set; }
    public string StrTags { get; set; }
    public int Point { get; set; }
    public decimal PriceWorth { get; set; }
    public int ShowStock { get; set; }
    public int Stock { get; set; }
    public int Sort { get; set; }
    public string StrInfos { get; set; }
    public string Operator { get; set; }
}

public class UpdateGiftParams
{
    public UpdateGiftParams(UpdateGiftRequest req, string @operator)
    {
        GiftId = req.GiftId;
        Status = (int) req.Status;
        CategoryId = req.CategoryId;
        StrTags = JsonConvert.SerializeObject(req.Tags);
        Point = req.Point;
        PriceWorth = req.PriceWorth;
        ShowStock = (int) req.ShowStock;
        Stock = req.Stock;
        Sort = req.Sort;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
        Operator = @operator;
    }
    
    public int GiftId { get; set; }
    public int Status { get; set; }
    public int CategoryId { get; set; }
    public string StrTags { get; set; }
    public int Point { get; set; }
    public decimal PriceWorth { get; set; }
    public int ShowStock { get; set; }
    public int Stock { get; set; }
    public int Sort { get; set; }
    public string StrInfos { get; set; }
    public string Operator { get; set; }
}

public class CreateCategoryParams
{
    public CreateCategoryParams(CreateCategoryRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string StrInfos { get; set; }
}

public class UpdateCategoryParams
{
    public UpdateCategoryParams(UpdateCategoryRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        CategoryId = req.CategoryId;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
    }
    
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int CategoryId { get; set; }
    public string StrInfos { get; set; }
}

public class CreateTagParams
{
    public CreateTagParams(CreateTagRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string StrInfos { get; set; }
}

public class UpdateTagParams
{
    public UpdateTagParams(UpdateTagRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        TagId = req.TagId;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int TagId { get; set; }
    public string StrInfos { get; set; }
}


public class GetRedemptionLimitParams
{
    public GetRedemptionLimitParams(GetRedemptionLimitRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
}

public class SetRedemptionLimitParams
{
    public SetRedemptionLimitParams(SetRedemptionLimitRequest req, string @operator)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        Status = (int) req.Status;
        Limit = req.Limit;
        Operator = @operator;
    }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int Status { get; set; }
    public int Limit { get; set; }
    public string Operator { get; set; }
}
