namespace StoreService.Core.Service.Model.BaseData;

public class GetTpCategoryListRequest
{
    public int SiteId { get; set; }
}

public class GetVipLevelListRequest
{
    public int SiteId { get; set; }
}

public class GetTpGameTypeListRequest
{
    public int SiteId { get; set; }
    public int TpCategoryId { get; set; }
}