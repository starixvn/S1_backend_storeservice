using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.GiftManagement;
using StoreService.Core.Service.Model.Redemption;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class RedemptionController : ControllerBase
{
    private readonly RedemptionBLL _redemptionBll;
    private readonly GiftManagementBLL _giftManagementBll;
    
    public RedemptionController(IServiceProvider provider)
    {
        _redemptionBll = provider.GetRequiredService<RedemptionBLL>();
        _giftManagementBll = provider.GetRequiredService<GiftManagementBLL>();
    }
    
    #region Redemption

    [HttpPost]
    public async Task<ApiResult> CreateRedemptionAsync(CreateRedemptionRequest request)
    {
        var result = new ApiResult();
        var redemptionLimit = _redemptionBll.CheckCustRedemptionLimitAsync(request.SiteId , request.CurrencyId , request.CustId);
        
        switch (redemptionLimit.Result)
        {
            case (int) ErrorCodeEnum.RedemptionLimitIsClose:
                result.ErrorCode = (int) ErrorCodeEnum.RedemptionLimitIsClose;
                break;
            case (int) ErrorCodeEnum.RedemptionLimitOverLimit:
                result.ErrorCode = (int) ErrorCodeEnum.RedemptionLimitOverLimit;
                break;
            case (int) GeneralStatusEnum.Enabled:
            {
                var success = await _redemptionBll.CreateRedemptionAsync(new CreateRedemptionParams(request));
                result.ErrorCode = (int) (success ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed);
                break;
            }
        }
        
        return result;
    }

    [HttpPost]
    public async Task<ApiResult> GetRedemptionListAsync(GetRedemptionListRequest request)
    {
        var result =
            await _redemptionBll.GetRedemptionListAsync(request);
        
        return new ApiListResult<GetRedemptionListResult>
        {
            Data = result.Datas,
            PageNumber = request.PageNumber,
            TotalPages = result.TotalPages,
            RowCount = result.RowCount,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    public async Task<ApiResult> ExportRedemptionListAsync(ExportRedemptionListRequest request)
    {
        var result =
            await _redemptionBll.ExportRedemptionListAsync(request);
        
        return new ApiResult<IEnumerable<GetRedemptionListResult>>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> GetRedemptionDetailAsync(GetRedemptionDetailRequest request)
    {
        var result =
            await _redemptionBll.GetRedemptionDetailAsync(request);
        
        return new ApiResult<GetRedemptionDetail>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> SetRedemptionInfoAsync(SetRedemptionInfoRequest request)
    {
        var result = new ApiResult();
        var success = false;
        
        var redemptionInfo = await _redemptionBll.GetRedemptionByTransIdAsync(request.TransId);
        
        if (redemptionInfo == null)
        {
            result.ErrorCode = (int) ErrorCodeEnum.Failed;
            result.RawMessage = "TransId doesn't exist";
            return result;
        }

        if (redemptionInfo.Status is (int) RedemptionEnum.Approved or (int) RedemptionEnum.Rejected)
        {
            request.ActualCost = redemptionInfo.ActualCost;
        }
        
        switch (request.Action)
        {
            case (int) RedemptionEnum.Draft:
                success = await _redemptionBll.SetRedemptionDraftAsync(request,redemptionInfo);
                break;
            
            case (int) RedemptionEnum.Pending:
                if (redemptionInfo.Status is (int) RedemptionEnum.OnHold or (int) RedemptionEnum.Draft
                    && redemptionInfo.Status != (int) RedemptionEnum.Pending)
                {
                    success = await _redemptionBll.SetRedemptionPendingAsync(request,redemptionInfo);
                }
                else
                {
                    result.RawMessage = "Pending Flow Is Invalid";
                }
                break;
            
            case (int) RedemptionEnum.Approved:
                if (redemptionInfo.Status is (int) RedemptionEnum.Pending or (int) RedemptionEnum.Draft
                    && redemptionInfo.Status != (int) RedemptionEnum.Approved)
                {
                    success = await _redemptionBll.SetRedemptionApprovedAsync(request,redemptionInfo);
                }
                else
                {
                    result.RawMessage = "Approved Flow Is Invalid";
                }
                break;
            
            case (int) RedemptionEnum.Rejected:
                if (redemptionInfo.Status is (int) RedemptionEnum.Pending or (int) RedemptionEnum.Draft
                    && redemptionInfo.Status != (int) RedemptionEnum.Rejected)
                {
                    success = await _redemptionBll.SetRedemptionRejectedAsync(request,redemptionInfo);
                }
                else
                {
                    result.RawMessage = "Rejected Flow Is Invalid";
                }
                break;
            
            case (int) RedemptionEnum.OnHold:
                if (redemptionInfo.Status is (int) RedemptionEnum.Pending or (int) RedemptionEnum.Draft 
                    && redemptionInfo.Status != (int) RedemptionEnum.OnHold)
                {
                    success = await _redemptionBll.SetRedemptionOnHoldAsync(request,redemptionInfo);
                }
                else
                {
                    result.RawMessage = "OnHold Flow Is Invalid";
                }
                break;
            
            default:
                result.RawMessage = "Request Action Is Invalid";
                break;
        }

        if (success)
        {
            result.ErrorCode = (int) ErrorCodeEnum.Success;
        }
        else
        {
            result.ErrorCode = (int) ErrorCodeEnum.Failed;
        }
        
        return result;
    }
    
    [HttpPost]
    public async Task<ApiResult> GetRedemptionLogAsync(GetRedemptionLogRequest request)
    {
        var result =
            await _redemptionBll.GetRedemptionLogAsync(request);
        
        return new ApiResult<IEnumerable<GetRedemptionLog>>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    #endregion
}