using Newtonsoft.Json;

using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Model.Redemption;

public class GetRedemptionListResult
{
    private DateTimeOffset _createTime { get; set; }
    private DateTimeOffset _updateTime { get; set; }

    public int SiteId { get; set; }
    public string IsoCode { get; set; }
    public int RedemptionId { get; set; }
    public string TransId { get; set; }
    public RedemptionEnum Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTimeOffset CreateTime
    {
        get => _createTime;
        set => _createTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset UpdateTime
    {
        get => _updateTime;
        set => _updateTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public string UserName { get; set; }
    public string SabaId { get; set; }
    public int VipLevel { get; set; }
    public string VipLevelName { get; set; }
    public string ItemName { get; set; }
    public int Point { get; set; }
    public decimal ActualCost { get; set; }
    public string Operator { get; set; }
    public string BonusCode { get; set; }
}

public class RedemptionListResult
{
    public IEnumerable<GetRedemptionOverviewResult> Overview { get; set; }
    public PagerResponseModel<GetRedemptionListResult> List { get; set; }
}

public class ExportRedemptionListResult
{
    public IEnumerable<GetRedemptionOverviewResult> Overview { get; set; }
    public IEnumerable<GetRedemptionListResult> List { get; set; }
}

public class GetRedemptionOverviewResult
{
    public int CurrencyId { get; set; }
    public string IsoCode { get; set; }
    public decimal TotalPoint { get; set; }
    public decimal TotalCost { get; set; }
    public int TotalRecord { get; set; }
}

public class GetRedemptionByTransIdResult
{
    private DateTimeOffset _createTime { get; set; }
    private DateTimeOffset _updateTime { get; set; }

    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int RedemptionId { get; set; }
    public int CustId { get; set; }
    public string TransId { get; set; }
    public int Status { get; set; }
    public decimal ActualCost { get; set; }

    public DateTimeOffset CreateTime
    {
        get => _createTime;
        set => _createTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset UpdateTime
    {
        get => _updateTime;
        set => _updateTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public int Point { get; set; }
    public string Operator { get; set; }
}

public class GetRedemptionDetail
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string UserName { get; set; }
    public decimal PointBalance { get; set; }
    public int RedemptionId { get; set; }
    public string TransId { get; set; }
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public int RedeemPoint { get; set; }
    public decimal PriceWorth { get; set; }
    public decimal ActualCost { get; set; }
    public int Status { get; set; }
    [JsonIgnore]
    private string ReceiptUrlString { get; set; }
    public IEnumerable<string> ReceiptUrls
        => JsonConvert.DeserializeObject<IEnumerable<string>>(ReceiptUrlString ?? "[]");
    public string AreaCode { get; set; }
    public string PhoneNo { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string PostCode { get; set; }
    public string LineFirst { get; set; }
    public string LineSecond { get; set; }
    public string AccountName { get; set; }
}

public class GetRedemptionLog
{
    private DateTimeOffset _createTime { get; set; }

    public int Action { get; set; }
    public string Comment { get; set; }
    public string Operator { get; set; }
    public DateTimeOffset CreateTime
    {
        get => _createTime;
        set => _createTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}

public class MyPurchesResult
{
    public PagerResponseModel<GetMyPurchesResult> List { get; set; }
}

public class GetMyPurchesResult
{
    private DateTimeOffset _createTime { get; set; }
    private DateTimeOffset _createTimeInCurrencyTimeZone { get; set; }

    private string _giftId { get; set; }

    public int SiteId { get; set; }
    public int CustId { get; set; }
    public int CurrencyId { get; set; }
    public string IsoCode { get; set; }
    public string TransId { get; set; }
    public string GiftId
    {
        get => _giftId;
        set => _giftId = "N" + value;
    }
    public string GiftName { get; set; }
    public int Point { get; set; }
    public RedemptionEnum Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTimeOffset CreateTime
    {
        get => _createTime;
        set => _createTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }

    public string TimeZone { get; set; }

    public DateTimeOffset CreateTimeInCurrencyTimeZone
    {
        get => _createTimeInCurrencyTimeZone;
        set
        {
            if (this.TimeZone.Contains("-"))
            {
                _createTimeInCurrencyTimeZone = new DateTimeOffset(value.DateTime, TimeSpan.Parse(this.TimeZone));
            }
            else
            {
                _createTimeInCurrencyTimeZone = new DateTimeOffset(value.DateTime, TimeSpan.Parse(this.TimeZone.Substring(1)));
            }
        }
    }
}

