using StoreService.Core.Service.Model.Member;

namespace StoreService.Core.Service.Model.RewardPoint;

public class CreateManualRewardPointInfoParams
{
    public CreateManualRewardPointInfoParams(CreateManualRewardPointInfoRequest req)
    {
        SiteId = req.SiteId;
        UserName = req.UserName;
        Point = req.Point;
        CategoryId = req.CategoryId;
        Memo = req.Memo;
        Operator = req.Operator;
    }
    
    public int SiteId { get; set; }
    public string UserName { get; set; }
    public decimal Point { get; set; }
    public int CategoryId { get; set; }
    public string Memo { get; set; }
    public string Operator { get; set; }
}

public class CreateManualRewardPointInfoBatchParams
{
    public CreateManualRewardPointInfoBatchParams(CreateManualRewardPointInfoBatchRequest req)
    {
        SiteId = req.SiteId;
        Infos = req.Infos;
        Operator = req.Operator;
    }
    
    public int SiteId { get; set; }
    public IEnumerable<ManualRewardPointInfo> Infos { get; set; }
    public string Operator { get; set; }
}

public class ManualRewardPointInfo
{
    public int CustId { get; set; }
    public int CurrencyId { get; set; }
    public int CategoryId { get; set; }
    public decimal Point { get; set; }
    public string Memo { get; set; }
}

public class BatchRewardPointLogInput
{
    public int CustId { get; set; }
    public int CurrencyId { get; set; }
    public string TransId { get; set; }
    public decimal ChangePoint { get; set; }
    public decimal BeforePoint { get; set; }
    public decimal AfterPoint { get; set; }
    public string Memo { get; set; }
}

public class DepositBatchRewardPointParams
{
    public int CurrencyId { get; set; }
    public IEnumerable<DepositRewardPointQueueInfo> DepositQueueInfos { get; set; }
    public IEnumerable<DepositRewardPointInfo> DepositRewardPointInfos { get; set; }
}

public class DepositRewardPointQueueInfo
{
    public string TransId { get; set; }
    public decimal RemainAmount { get; set; }
    public int Status { get; set; }
}

public class DepositRewardPointInfo
{
    public string TransId { get; set; }
    public int CustId { get; set; }
    public decimal Point { get; set; }
    public int SiteId { get; set; }
}