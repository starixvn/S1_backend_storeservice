
namespace StoreService.Core.Service.Model.Redemption;

public class CreateRedemptionParams
{
    public CreateRedemptionParams(CreateRedemptionRequest req)
    {
        CustId = req.CustId;
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        GiftId = req.GiftId;
        Point = req.Point;
    }
    public int CustId { get; set; }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int GiftId { get; set; }
    public int Point { get; set; }
}

public class SetRedemptionLogParams
{
    public int SiteId { get; set; }
    public string TransId { get; set; }
    public int Action { get; set; }
    public string Comment { get; set; }
    public string Operator { get; set; }
}