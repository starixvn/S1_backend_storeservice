using Newtonsoft.Json;

namespace StoreService.Core.Service.Model.Frontend;


public class FrontendGetListParams
{
    public FrontendGetListParams(GetListRequest req)
    {
        CustId = req.CustId;
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        UseCategoryIds = req.CategoryIds != null;
        StrCategoryIds = req.CategoryIds == null ? "[]" : JsonConvert.SerializeObject(req.CategoryIds);
        UseTags = req.Tags != null;
        StrTags = req.Tags == null ? "[]" : JsonConvert.SerializeObject(req.Tags);
        Lang = req.LangId;
        OrderByField = string.IsNullOrWhiteSpace(req.OrderByField) ? "Sort" : req.OrderByField;
        Desc = req.Desc;
        PageSize = req.PageSize;
    }
    public int CustId { get; set; }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public bool UseCategoryIds { get; set; }
    public string StrCategoryIds { get; set; }
    public bool UseTags { get; set; }
    public string StrTags { get; set; }
    public string Lang { get; set; }
    public string OrderByField { get; set; }
    public bool Desc { get; set; }
    public int PageSize { get; set; }
}

