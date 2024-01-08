namespace StoreService.Core.Service.Model.TurnoverPeriod;

public class TurnoverPeriod
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int RedeemHours { get; set; }
    public DateTime LastSettledTime { get; set; }
}


