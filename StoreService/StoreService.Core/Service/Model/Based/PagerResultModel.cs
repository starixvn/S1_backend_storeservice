namespace StoreService.Core.Service.Model.Based;

public class PagerResultModel<T>
{
    public IEnumerable<T> Datas { get; set; }
    public int TotalPages { get; set; }
    public int RowCount { get; set; }
}

public class PagerResponseModel<T>
{
    public IEnumerable<T> Datas { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int RowCount { get; set; }
}