using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Model.Member;

public class MemberRewardPointInfo
{
    private DateTimeOffset _lastUpdateTime { get; set; }
    private DateTimeOffset _lastRedemptionTime { get; set; }

    public int CustId { get; set; }
    public string IsoCode { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public string VipLevelName { get; set; }
    public decimal PointBalance { get; set; }
    public decimal PointEarned { get; set; }
    public decimal PointUsed { get; set; }
    public DateTimeOffset LastUpdateTime
    {
        get => _lastUpdateTime;
        set => _lastUpdateTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset LastRedemptionTime
    {
        get => _lastRedemptionTime;
        set => _lastRedemptionTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}

public class MemberRewardPointLog : LastOperateModel
{
    public PointTypeEnum PointType { get; set; }
    public string TypeName => PointType.ToString();
    public string RefTransId { get; set; }
    public decimal ChangePoint { get; set; }
    public decimal BeforePoint { get; set; }
    public decimal AfterPoint { get; set; }
    public string Memo { get; set; }
}

public class BatchManualRewardPointResult
{
    public string TransId { get; set; }
    public int CustId { get; set; }
    public int CurrencyId { get; set; }
    public decimal Point { get; set; }
    public string Memo { get; set; }
}

public class BatchChangeCustRewardPointResult
{
    public int CustId { get; set; }
    public decimal BeforePoint { get; set; }
    public decimal AfterPoint { get; set; }
}

public class MemberPointBalance
{
    public decimal PointBalance { get; set; }
}