using Newtonsoft.Json;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Model.GiftManagement;

public class Gift : LastUpdateModel
{
    public int GiftId { get; set; }
    public string GiftName { get; set; }
    public int SiteId { get; set; }
    public string SiteName { get; set; }
    public int CurrencyId { get; set; }	
    public string IsoCode { get; set; }
    public int CategoryId { get; set; }
    public string Lang { get; set; }
    public string Description { get; set; }
    public string PictureUrl { get; set; }
    public GeneralStatusEnum Status { get; set; }
    [JsonIgnore]
    public string TagIds { get; set; }
    public IEnumerable<GiftTag> Tags { get; set; }
    public int Point { get; set; }
    public decimal PriceWorth { get; set; }
    public bool ShowStock { get; set; }
    public int Stock { get; set; }
    public int Sort { get; set; }
}

public class GiftInfo
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int GiftId { get; set; }
    public string GiftName { get; set; }
    public int CategoryId { get; set; }
    public string Lang { get; set; }
    public string Description { get; set; }
    public string PictureUrl { get; set; }
    public GeneralStatusEnum Status { get; set; }
    [JsonIgnore]
    public string Tags { get; set; }
    public IEnumerable<int> TagIds
        => JsonConvert.DeserializeObject<IEnumerable<int>>(Tags ?? "[]");
    
    public int Point { get; set; }
    public decimal PriceWorth { get; set; }
    public bool ShowStock { get; set; }
    public int Stock { get; set; }
    public int Sort { get; set; }
}

public class GiftCategory
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Lang { get; set; }
}

public class GiftTag
{
    public int TagId { get; set; }
    public string TagName { get; set; }
    public string Lang { get; set; }
}

public class RedemptionLimit : LastUpdateModel
{
    public int Limit { get; set; }
    public GeneralStatusEnum Status { get; set; }
}
