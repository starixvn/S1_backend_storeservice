using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.GiftManagement;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class GiftManagementController : ControllerBase
{
    private readonly GiftManagementBLL _giftManagementBll;
    
    public GiftManagementController(IServiceProvider provider)
    {
        _giftManagementBll = provider.GetRequiredService<GiftManagementBLL>();
    }

    [HttpPost]
    public async Task<ApiResult> GetListAsync(GetListRequest request)
    {
        var result =
            await _giftManagementBll.GetGiftListAsync(new GetListParams(request));

        return new ApiResult<IEnumerable<Gift>>
        {
            Data = result,
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
    public async Task<ApiResult> GetAsync(GetGiftRequest request)
    {
        var result =
            await _giftManagementBll.GetGiftAsync(new GetGiftParams(request));

        return new ApiResult<IEnumerable<GiftInfo>>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> CreateAsync(CreateGiftRequest request)
    {
        var result =
            await _giftManagementBll.CreateGiftAsync(new CreateGiftParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> UpdateAsync(UpdateGiftRequest request)
    {
        var result =
            await _giftManagementBll.UpdateGiftAsync(new UpdateGiftParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var result =
            await _giftManagementBll.CreateCategoryAsync(new CreateCategoryParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var result =
            await _giftManagementBll.UpdateCategoryAsync(new UpdateCategoryParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> CreateTagAsync(CreateTagRequest request)
    {
        var result =
            await _giftManagementBll.CreateTagAsync(new CreateTagParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> UpdateTagAsync(UpdateTagRequest request)
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
        var result =
            await _giftManagementBll.SetRedemptionLimitAsync(new SetRedemptionLimitParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetSortAsync(SetSortRequest request)
    {
        var result =
            await _giftManagementBll.SetGiftSortAsync(new SetSortParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetShoppingCartAsync(SetShoppingCartRequest request)
    {
        var success = true;
        if (request.GiftId.ToList().Count > 0)
        {
            success = await _giftManagementBll.SetShoppingCartAsync(request);
        }
        
        if (success)
        {
            var result = await _giftManagementBll.GetShoppingCartAsync(request);
            return new ApiResult<IEnumerable<Gift>>
            {
                Data = result,
                ErrorCode = (int) ErrorCodeEnum.Success
            };
        }

        return new ApiResult<IEnumerable<Gift>>
        {
            ErrorCode = (int) ErrorCodeEnum.Failed
        };
        
    }
}