namespace StoreService.Core.Service.Model.Based;

public class LastOperateModel
{
    private DateTimeOffset _createTime { get; set; }

    public string Operator { get; set; }
    public DateTimeOffset CreateTime
    {
        get => _createTime;
        set => _createTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}