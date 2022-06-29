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