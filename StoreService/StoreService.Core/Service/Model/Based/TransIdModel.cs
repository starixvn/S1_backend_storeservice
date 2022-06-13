namespace StoreService.Core.Service.Model.Based;

public class TransIdModel
{
    public TransIdModel(string transId)
    {
        TransId = transId;
    }
    public string TransId { get; set; }
}