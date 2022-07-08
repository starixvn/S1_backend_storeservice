namespace StoreService.Core.Service.Model;

public class SupportSetting
{
    public int SiteId { get; set; }
    public IEnumerable<int> CurrencyIds { get; set; }
}