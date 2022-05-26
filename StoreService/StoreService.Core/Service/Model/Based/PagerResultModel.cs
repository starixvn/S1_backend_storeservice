namespace StoreService.Core.Service.Model.Based;

public class PagerResultModel<T>
{
    public IEnumerable<T> Datas { get; set; }
    public int TotalPages { get; set; }
    public int RowCount { get; set; }
}