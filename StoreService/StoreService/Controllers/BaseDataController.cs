using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.BaseData;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BaseDataController : ControllerBase
{
    private readonly CommonBLL _commonBll;
    
    public BaseDataController(IServiceProvider provider)
    {
        _commonBll = provider.GetRequiredService<CommonBLL>();
    }
    
    [HttpPost]
    public async Task<ApiResult> GetTpCategoryListAsync()
    {
        var result =
            await _commonBll.GetTpCategoryAsync();

        return new ApiListResult<TpCategory>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
}