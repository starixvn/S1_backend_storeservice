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

public class DepositPointSetting
{ 
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }
    
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PointRate { get; set; }
    public bool PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }
    public DateTimeOffset PromotionStartTime
    {
        get => _startTime;
        set => _startTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset PromotionEndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}