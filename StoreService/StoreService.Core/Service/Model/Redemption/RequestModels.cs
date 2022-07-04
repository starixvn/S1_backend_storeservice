using StoreService.Core.Service.Model.Based;

namespace StoreService.Core.Service.Model.Redemption;

public class CreateRedemptionRequest
{
    public int CustId { get; set; }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int GiftId { get; set; }
    public int Point { get; set; }
}

public class GetRedemptionListRequest : PagerRequestModel
{
    private DateTimeOffset? _startTime;
    private DateTimeOffset? _endTime;
    public DateTimeOffset? StartTime 
    {
        get => _startTime;
        set
        {
            if (value != null) 
                _startTime = new DateTimeOffset(value.Value.DateTime, TimeSpan.Zero);
        }
    }
    public DateTimeOffset? EndTime 
    {
        get => _endTime;
        set
        {
            if (value != null) 
                _endTime = new DateTimeOffset(value.Value.DateTime, TimeSpan.Zero);
        }
    }
    public IEnumerable<int> SiteIds { get; set; }
    public IEnumerable<int> CurrencyIds { get; set; }
    public IEnumerable<int> VipLevels { get; set; }
    public IEnumerable<int> Statuses { get; set; }
    
    //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    //[JsonIgnore]
    public string TransId { get; set; }
    //[JsonIgnore]
    //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string UserName { get; set; }
    //[JsonIgnore]
    //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string SabaId { get; set; }
}
