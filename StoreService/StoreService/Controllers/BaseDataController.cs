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
    public async Task<ApiResult> GetTpCategoryListAsync(GetTpCategoryListRequest request)
    {
        var result =
            await _commonBll.GetTpCategoryListAsync(new GetTpCategoryListParams(request));

        return new ApiListResult<TpCategory>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetTpGameTypeListAsync(GetTpGameTypeListRequest request)
    {
        var result =
            await _commonBll.GetTpGameTypeListAsync(new GetTpGameTypeListParams(request));

        return new ApiListResult<TpGameType>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetVipLevelListAsync(GetVipLevelListRequest request)
    {
        var result =
            await _commonBll.GetVipLevelListAsync(new GetVipLevelListParams(request));

        return new ApiListResult<VipLevel>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
}