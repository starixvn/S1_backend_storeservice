using Newtonsoft.Json;
using StoreService.Core.Extensions;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Report;

namespace StoreService.Core.Service.Model.Redemption;

public class GetRedemptionListParams: PagerParamModel
{
    public GetRedemptionListParams(GetRedemptionListRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        StrStatuses = JsonConvert.SerializeObject(req.Statuses);
        TransId = req.TransId ?? string.Empty;
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.SabaId ?? string.Empty;
        GiftName = string.Empty;
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
        PageNumber = req.PageNumber;
    }
    
    public GetRedemptionListParams(GetRedemptionReportRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        StrStatuses = "[2]"; // 2: Approved
        TransId = string.Empty;
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.TpUserName ?? string.Empty;
        GiftName = req.GiftName ?? string.Empty;
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
        PageNumber = req.PageNumber;
    }
   
    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrVipLevels { get; set; }
    public string StrStatuses { get; set; }
    public string TransId { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public string GiftName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PageNumber { get; set; }
}

public class CreateRedemptionParams
{
    public CreateRedemptionParams(CreateRedemptionRequest req)
    {
        CustId = req.CustId;
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        GiftId = req.GiftId;
        Point = req.Point;
    }
    public int CustId { get; set; }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int GiftId { get; set; }
    public int Point { get; set; }
}

public class SetRedemptionLogParams
{
    public int SiteId { get; set; }
    public string TransId { get; set; }
    public int Action { get; set; }
    public string Comment { get; set; }
    public string Operator { get; set; }
}