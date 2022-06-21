using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.PointSetting;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PointSettingController : ControllerBase
{
    private readonly PointSettingBLL _pointSettingBll;
    
    public PointSettingController(IServiceProvider provider)
    {
        _pointSettingBll = provider.GetRequiredService<PointSettingBLL>();
    }
    
    [HttpPost]
    public async Task<ApiResult> GetInactivePointExpirySettingAsync(GetInactivePointExpirySettingRequest request)
    {
        var result =
            await _pointSettingBll.GetInactivePointExpirySettingAsync(new GetInactivePointExpirySettingParams(request));

        return new ApiResult<InactivePointExpirySetting>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetInactivePointExpirySettingAsync(SetInactivePointExpirySettingRequest request)
    {
        var result =
            await _pointSettingBll.SetInactivePointExpirySettingAsync(new SetInactivePointExpirySettingParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetTurnoverPointSettingListAsync(GetTurnoverPointSettingListRequest request)
    {
        var result =
            await _pointSettingBll.GetTurnoverPointSettingListAsync(new GetTurnoverPointSettingListParams(request));

        return new ApiListResult<TurnoverPointSettingResult>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetTurnoverPointSettingAsync(GetTurnoverPointSettingRequest request)
    {
        var result =
            await _pointSettingBll.GetTurnoverPointSettingAsync(new GetTurnoverPointSettingParams(request));

        return new ApiListResult<TurnoverPointSetting>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetTurnoverPointSettingAsync(SetTurnoverPointSettingRequest request)
    {
        var result =
            await _pointSettingBll.SetTurnoverPointSettingAsync(new SetTurnoverPointSettingParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetDepositPointSettingAsync(GetDepositPointSettingRequest request)
    {
        var result =
            await _pointSettingBll.GetDepositPointSettingAsync(new GetDepositPointSettingParams(request));

        return new ApiListResult<DepositPointSetting>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetDepositPointSettingAsync(SetDepositPointSettingRequest request)
    {
        var result =
            await _pointSettingBll.SetDepositPointSettingAsync(new SetDepositPointSettingParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetTurnoverPointStatusSettingAsync(GetTurnoverPointStatusSettingRequest request)
    {
        var result =
            await _pointSettingBll.GetTurnoverPointStatusSettingAsync(new GetTurnoverPointStatusSettingParams(request));

        return new ApiListResult<TurnoverPointStatusSetting>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetTurnoverPointStatusSettingAsync(SetTurnoverPointStatusSettingRequest request)
    {
        var result =
            await _pointSettingBll.SetTurnoverPointStatusSettingAsync(new SetTurnoverPointStatusSettingParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
}