namespace StoreService.Core.Service.Model.Report;

public class GetDepositPointReportResult
{
    private DateTimeOffset _createTime { get; set; }
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string IsoCode { get; set; }
    public string UserName { get; set; }
    public string SabaId { get; set; }
    public int VipLevel { get; set; }
    public string VipLevelName { get; set; }
    public string TransId { get; set; }
    public decimal Amount { get; set; }
    public decimal PointEarned { get; set; }
    public string Memo { get; set; }
    public string Operator { get; set; }
    public DateTimeOffset CreateTime
    {
        get => _createTime;
        set => _createTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}