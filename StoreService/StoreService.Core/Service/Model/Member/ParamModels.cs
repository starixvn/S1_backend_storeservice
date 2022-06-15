using Newtonsoft.Json;
using StoreService.Core.Extensions;
using StoreService.Core.Service.Model.Based;

namespace StoreService.Core.Service.Model.Member;

public class GetListParams : PagerParamModel
{
    public GetListParams(GetListRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
    }
    
    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrVipLevels { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class GetMemberRewardPointLogParams : PagerParamModel
{
    public GetMemberRewardPointLogParams(GetMemberRewardPointLogRequest req)
    {
        CustId = req.CustId;
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
    }
    
    public int CustId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

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