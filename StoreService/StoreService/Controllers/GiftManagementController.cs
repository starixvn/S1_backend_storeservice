using Microsoft.AspNetCore.Mvc;
using StoreService.Service.BLL;
using StoreService.Service.Model;
using StoreService.Service.Model.Enums;
using StoreService.Service.Model.GiftManagement;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
public class GiftManagementController : ControllerBase
{
    private readonly GiftManagementBLL _giftManagementBll;
    
    public GiftManagementController(IServiceProvider provider)
    {
        _giftManagementBll = provider.GetRequiredService<GiftManagementBLL>();
    }
    
    [HttpPost]
    public ApiResult Test()
    {
        return new ApiResult
        {
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }

    [HttpPost]
    public async Task<ApiResult> GetListAsync(GetListRequest request)
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
    }
    
    [HttpPost]
    public async Task<ApiResult> GetTagsAsync(GetTagsRequest request)
    {
        var result =
            await _giftManagementBll.GetGiftTagsAsync(new GetTagsParams(request));

        return new ApiListResult<GiftTag>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetCategoriesAsync(GetCategoriesRequest request)
    {
        var result =
            await _giftManagementBll.GetGiftCategoriesAsync(new GetCategoriesParams(request));

        return new ApiListResult<GiftCategory>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetRedemptionLimitAsync(GetRedemptionLimitRequest request)
    {
        var result =
            await _giftManagementBll.GetRedemptionLimitAsync(new GetRedemptionLimitParams(request));

        return new ApiResult<RedemptionLimit>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> CreateAsync(CreateGiftRequest request)
    {
        var user = "System";
        var result =
            await _giftManagementBll.CreateGiftAsync(new CreateGiftParams(request, user));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> UpdateAsync(UpdateGiftRequest request)
    {
        var user = "System";
        var result =
            await _giftManagementBll.UpdateGiftAsync(new UpdateGiftParams(request, user));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> CreateAsync(CreateCategoryRequest request)
    {
        var result =
            await _giftManagementBll.CreateCategoryAsync(new CreateCategoryParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> UpdateAsync(UpdateCategoryRequest request)
    {
        var result =
            await _giftManagementBll.UpdateCategoryAsync(new UpdateCategoryParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> CreateAsync(CreateTagRequest request)
    {
        var result =
            await _giftManagementBll.CreateTagAsync(new CreateTagParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> UpdateAsync(UpdateTagRequest request)
    {
        var result =
            await _giftManagementBll.UpdateTagAsync(new UpdateTagParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetRedemptionLimitAsync(SetRedemptionLimitRequest request)
    {
        var user = "System";
        var result =
            await _giftManagementBll.SetRedemptionLimitAsync(new SetRedemptionLimitParams(request, user));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
}