namespace StoreService.Service.Model.Based;

public class LastUpdateModel
{
    private DateTimeOffset _lastUpdateTime { get; set; }
    public DateTimeOffset LastUpdateTime
    {
        get => _lastUpdateTime;
        set => _lastUpdateTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}