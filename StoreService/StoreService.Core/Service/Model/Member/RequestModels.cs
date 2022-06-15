using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Model.Member;

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
        set => _startTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset EndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
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
        set => _startTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset EndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
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


