using Newtonsoft.Json;

using StoreService.Core.Extensions;
using StoreService.Core.Service.Model.Based;

namespace StoreService.Core.Service.Model.Report;

#region DepositPoint

public class GetDepositPointReportParams : PagerParamModel
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

    public GetDepositPointReportParams(ExportDepositPointReportParams input)
    {
        SiteId = input.SiteId;
        StrCurrencyIds = input.StrCurrencyIds;
        StrVipLevels = input.StrVipLevels;
        TransId = input.TransId;
        UserName = input.UserName;
        TpUserName = input.TpUserName;
        StartTime = input.StartTime;
        EndTime = input.EndTime;
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



public class ExportDepositPointReportParams
{
    public ExportDepositPointReportParams(ExportDepositPointReportRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        TransId = req.TransId ?? string.Empty;
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.SabaId ?? string.Empty;
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
    }

    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrVipLevels { get; set; }
    public string TransId { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

#endregion

#region AdjustmentPoint

public class GetAdjustmentPointReportParams : PagerParamModel
{
    public GetAdjustmentPointReportParams(GetAdjustmentPointReportRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        StrAdjustTypes = JsonConvert.SerializeObject(req.AdjustTypes);
        TransId = req.TransId ?? string.Empty;
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.SabaId ?? string.Empty;
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
        PageNumber = req.PageNumber;
    }

    public GetAdjustmentPointReportParams(ExportAdjustmentPointReportParams input)
    {
        SiteId = input.SiteId;
        StrCurrencyIds = input.StrCurrencyIds;
        StrVipLevels = input.StrVipLevels;
        StrAdjustTypes = input.StrAdjustTypes;
        TransId = input.TransId;
        UserName = input.UserName;
        TpUserName = input.TpUserName;
        StartTime = input.StartTime;
        EndTime = input.EndTime;
    }

    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrAdjustTypes { get; set; }
    public string StrVipLevels { get; set; }
    public string TransId { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PageNumber { get; set; }
}



public class ExportAdjustmentPointReportParams
{
    public ExportAdjustmentPointReportParams(ExportAdjustmentPointReportRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        StrAdjustTypes = JsonConvert.SerializeObject(req.AdjustTypes);
        TransId = req.TransId ?? string.Empty;
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.SabaId ?? string.Empty;
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
    }

    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrVipLevels { get; set; }
    public string StrAdjustTypes { get; set; }
    public string TransId { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

#endregion

#region TurnoverPoint

public class GetTurnoverPointReportParams : PagerParamModel
{
    public GetTurnoverPointReportParams(GetTurnoverPointReportRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        StrCategories = JsonConvert.SerializeObject(req.Categories);
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.SabaId ?? string.Empty;
        PageSize = req.PageSize;
        StartIndex = req.PageSize.GetStartIndex(req.PageNumber);
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
        PageNumber = req.PageNumber;
    }

    public GetTurnoverPointReportParams(ExportTurnoverPointReportParams input)
    {
        SiteId = input.SiteId;
        StrCurrencyIds = input.StrCurrencyIds;
        StrVipLevels = input.StrVipLevels;
        StrCategories = input.StrCategories;
        UserName = input.UserName;
        TpUserName = input.TpUserName;
        StartTime = input.StartTime;
        EndTime = input.EndTime;
    }

    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrVipLevels { get; set; }
    public string StrCategories { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PageNumber { get; set; }
}



public class ExportTurnoverPointReportParams
{
    public ExportTurnoverPointReportParams(ExportTurnoverPointReportRequest req)
    {
        SiteId = req.SiteId;
        StrCurrencyIds = JsonConvert.SerializeObject(req.CurrencyIds);
        StrVipLevels = JsonConvert.SerializeObject(req.VipLevels);
        StrCategories = JsonConvert.SerializeObject(req.Categories);
        UserName = req.UserName ?? string.Empty;
        TpUserName = req.SabaId ?? string.Empty;
        StartTime = req.StartTime.DateTime;
        EndTime = req.EndTime.DateTime;
    }

    public int SiteId { get; set; }
    public string StrCurrencyIds { get; set; }
    public string StrVipLevels { get; set; }
    public string StrCategories { get; set; }
    public string UserName { get; set; }
    public string TpUserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

#endregion
