namespace StoreService.Core.Service.Model.RewardPoint;

public class DepositRewardPointQueue
{
    public string TransId { get; set; }
    public decimal RemainAmount { get; set; }
    public int CustId { get; set; }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public DateTime DepositUpdateTime { get; set; }
}