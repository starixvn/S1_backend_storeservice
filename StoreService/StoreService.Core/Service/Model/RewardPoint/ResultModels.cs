namespace StoreService.Core.Service.Model.RewardPoint;

public class DepositRewardPointQueue
{
    private DateTimeOffset _depositUpdateTime { get; set; }
    public string TransId { get; set; }
    public decimal RemainAmount { get; set; }
    public int CustId { get; set; }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public DateTimeOffset DepositUpdateTime
    {
        get => _depositUpdateTime;
        set => _depositUpdateTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}