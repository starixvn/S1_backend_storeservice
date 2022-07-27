using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreService.Core.Service.Model.Member;

public class GetBalanceRequest
{
    public int CustId { get; set; }
}

public class GetListRequest : PagerRequestModel
{
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }
    
    public int SiteId { get; set; }
    public IEnumerable<int> CurrencyIds { get; set; }
    public IEnumerable<int> VipLevels { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTimeOffset StartTime
    {
        get => _startTime;
        set => _startTime = new DateTimeOffset(value.ToUniversalTime().DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset EndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.ToUniversalTime().DateTime, TimeSpan.Zero);
    }
}

public class GetMemberRewardPointLogRequest : PagerRequestModel
{
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }
    
    public int CustId { get; set; }
    public DateTimeOffset StartTime
    {
        get => _startTime;
        set => _startTime = new DateTimeOffset(value.ToUniversalTime().DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset EndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.ToUniversalTime().DateTime, TimeSpan.Zero);
    }
}

public class CreateManualRewardPointInfoRequest
{
    public int SiteId { get; set; }
    public string UserName { get; set; }
    public decimal Point { get; set; }
    public int CategoryId { get; set; }
    public string Memo { get; set; }
    public string Operator { get; set; }
}

public class CreateManualRewardPointInfoBatchRequest
{
    public int SiteId { get; set; }
    public IEnumerable<ManualRewardPointInfo> Infos { get; set; }
    public string Operator { get; set; }
}

public class BatchRewardPointLogInfo
{
    public int CustId { get; set; }
    public int CurrencyId { get; set; }
    public int SiteId { get; set; }
    public string TransId { get; set; }
    public decimal ChangePoint { get; set; }
    public decimal BeforePoint { get; set; }
    public decimal AfterPoint { get; set; }
    public string Memo { get; set; }
}

public class BatchRewardPointLogInfoComparer : IEqualityComparer<BatchRewardPointLogInfo>
{
    public bool Equals(BatchRewardPointLogInfo t1, BatchRewardPointLogInfo t2)
    {
        return (t1.TransId == t2.TransId
                && t1.CustId == t2.CustId);
    }

    public int GetHashCode(BatchRewardPointLogInfo t)
    {
        return t.TransId.GetHashCode();
    }
}

public class SetFavoriteRequest
{
    public int CustId { get; set; }
    public int GiftId { get; set; }
    public bool IsFavorite { get; set; }
}

public class GetMyCoinRequest
{
    public int CustId { get; set; }
}