namespace StoreService.Core.Service.Model.BaseData;

public class GetTpCategoryListParams
{
    public GetTpCategoryListParams(GetTpCategoryListRequest req)
    {
        SiteId = req.SiteId;
    }
    
    public int SiteId { get; set; }
}

public class GetVipLevelListParams
{
    public GetVipLevelListParams(GetVipLevelListRequest req)
    {
        SiteId = req.SiteId;
    }
    
    public int SiteId { get; set; }
}

public class GetTpGameTypeListParams
{
    public GetTpGameTypeListParams(GetTpGameTypeListRequest req)
    {
        SiteId = req.SiteId;
        TpCategoryId = req.TpCategoryId;
    }
    
    public int SiteId { get; set; }
    public int TpCategoryId { get; set; }
}