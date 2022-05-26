namespace StoreService.Core.Service.Model.Based;

public class PagerRequestModel
{
    private int _pageNumber { get; set; }
    private int _pageSize { get; set; } = 10;
    
    public int PageNumber
    {
        get => _pageNumber;
        set => Math.Abs(_pageNumber);
    }

    public int PageSize
    {
        get => _pageSize;
        set => Math.Abs(_pageSize);
    }
}