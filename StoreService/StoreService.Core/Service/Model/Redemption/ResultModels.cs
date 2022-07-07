namespace StoreService.Core.Service.Model.Redemption;

public class GetRedemptionListResult
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int RedemptionId { get; set; }
    public string TransId { get; set; }
    public int Status { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public string UserName { get; set; }
    public string SabaId { get; set; }
    public int VipLevel { get; set; }
    public string VipLevelName { get; set; }
    public string ItemName { get; set; }
    public int Point { get; set; }
    public decimal ActualCost { get; set; }
    public string Operator { get; set; }
}

public class GetRedemptionByTransIdResult
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int RedemptionId { get; set; }
    public int CustId { get; set; }
    public string TransId { get; set; }
    public int Status { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
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
    public string ItemName { get; set; }
    public int RedeemPoint { get; set; }
    public decimal PriceWorth { get; set; }
    public decimal ActualCost { get; set; }
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

