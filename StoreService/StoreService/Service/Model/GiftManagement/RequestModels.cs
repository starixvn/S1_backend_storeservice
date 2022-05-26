using StoreService.Service.Model.Based;
using StoreService.Service.Model.Enums;

namespace StoreService.Service.Model.GiftManagement;

public class GetListRequest : PagerRequestModel
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public IEnumerable<int> Status { get; set; }
    public IEnumerable<int> CategoryIds { get; set; }
    public IEnumerable<int> Tags { get; set; }
    public string GiftName { get; set; }
}

public class GetTagsRequest
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public IEnumerable<int> LangCodes { get; set; }
}

public class GetCategoriesRequest
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public IEnumerable<int> LangCodes { get; set; }
}

public class CreateGiftRequest
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public GeneralStatusEnum Status { get; set; }
    public int CategoryId { get; set; }
    public IEnumerable<int> Tags { get; set; }
    public int Point { get; set; }
    public decimal PriceWorth { get; set; }
    public GeneralStatusEnum ShowStock { get; set; }
    public int Stock { get; set; }
    public int Sort { get; set; }
    public IEnumerable<LangInfoModel> Infos { get; set; }
}

public class UpdateGiftRequest
{
    public int GiftId { get; set; }
    public GeneralStatusEnum Status { get; set; }
    public int CategoryId { get; set; }
    public IEnumerable<int> Tags { get; set; }
    public int Point { get; set; }
    public decimal PriceWorth { get; set; }
    public GeneralStatusEnum ShowStock { get; set; }
    public int Stock { get; set; }
    public int Sort { get; set; }
    public IEnumerable<LangInfoModel> Infos { get; set; }
}

public class CreateCategoryRequest
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public IEnumerable<LangNameModel> Infos { get; set; }
}

public class UpdateCategoryRequest
{
    public int CategoryId { get; set; }
    public IEnumerable<LangNameModel> Infos { get; set; }
}

public class CreateTagRequest
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public IEnumerable<LangNameModel> Infos { get; set; }
}

public class UpdateTagRequest
{
    public int TagId { get; set; }
    public IEnumerable<LangNameModel> Infos { get; set; }
}


public class GetRedemptionLimitRequest
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
}


public class SetRedemptionLimitRequest
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public GeneralStatusEnum Status { get; set; }
    public int Limit { get; set; }
}