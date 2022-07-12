using Newtonsoft.Json;
using StoreService.Core.Extensions;
using StoreService.Core.Service.Model.Based;

namespace StoreService.Core.Service.Model.Report;

public class GetDepositPointReportParams: PagerParamModel
{
    public GetDepositPointReportParams(GetDepositPointReportRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        TransId = req.TransId ?? string.Empty;
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.SabaId ?? string.Empty;
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
        PageNumber = req.PageNumber;
    }
    
    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrVipLevels { get; set; }
    public string TransId { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PageNumber { get; set; }
}