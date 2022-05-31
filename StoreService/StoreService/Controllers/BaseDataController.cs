using Microsoft.AspNetCore.Mvc;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BaseDataController : ControllerBase
{
    //private readonly GiftManagementBLL _giftManagementBll;
    
    public BaseDataController(IServiceProvider provider)
    {
        //_giftManagementBll = provider.GetRequiredService<GiftManagementBLL>();
    }
    
    /*[HttpPost]
    public async Task<ApiResult> GetTpCategoryListAsync(GetListRequest request)
    {
        var result =
            await _giftManagementBll.GetGiftListAsync(new GetListParams(request));

        return new ApiListResult<Gift>
        {
            Data = result.Datas,
            PageNumber = request.PageNumber,
            TotalPages = result.TotalPages,
            RowCount = result.RowCount,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }*/
}