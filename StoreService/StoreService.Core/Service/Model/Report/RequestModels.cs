using StoreService.Core.Service.Model.Based;

namespace StoreService.Core.Service.Model.Report;

public class GetRedemptionReportRequest: PagerRequestModel
{
    private DateTimeOffset _startTime;
    private DateTimeOffset _endTime;
    public int SiteId { get; set; }
    public IEnumerable<int> CurrencyIds { get; set; }
    public IEnumerable<int> VipLevels { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public string GiftName { get; set; }
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

public class ExportRedemptionReportRequest
{
    private DateTimeOffset _startTime;
    private DateTimeOffset _endTime;
    public int SiteId { get; set; }
    public IEnumerable<int> CurrencyIds { get; set; }
    public IEnumerable<int> VipLevels { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public string GiftName { get; set; }
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

public class GetRedemptionOverviewRequest
{
    private DateTimeOffset _startTime;
    private DateTimeOffset _endTime;
    public int SiteId { get; set; }
    public IEnumerable<int> CurrencyIds { get; set; }
    public IEnumerable<int> VipLevels { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public string GiftName { get; set; }
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