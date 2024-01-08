using Microsoft.AspNetCore.Mvc;

using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class MemberController : ControllerBase
{
    private readonly MemberBLL _memberBll;
    private readonly RewardPointBLL _rewardPointBll;

    public MemberController(IServiceProvider provider)
    {
        _memberBll = provider.GetRequiredService<MemberBLL>();
        _rewardPointBll = provider.GetRequiredService<RewardPointBLL>();
    }

    [HttpPost]
    public async Task<ApiResult> GetBalanceAsync(GetBalanceRequest request)
    {
        var result =
            await _memberBll.GetMemberPointBalanceAsync(request.CustId);

        return new ApiResult<MemberPointBalance>
        {
            Data = result,
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    [HttpPost]
    public async Task<ApiResult> GetListAsync(GetListRequest request)
    {
        var result =
            await _memberBll.GetMemberRewardPointListAsync(new GetListParams(request));

        return new ApiListResult<MemberRewardPointInfo>
        {
            Data = result.Datas,
            PageNumber = request.PageNumber,
            TotalPages = result.TotalPages,
            RowCount = result.RowCount,
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    [HttpPost]
    public async Task<ApiResult> GetRewardPointLogAsync(GetMemberRewardPointLogRequest request)
    {
        var result =
            await _memberBll.GetMemberRewardPointLogAsync(new GetMemberRewardPointLogParams(request));

        return new ApiListResult<MemberRewardPointLog>
        {
            Data = result.Datas,
            PageNumber = request.PageNumber,
            TotalPages = result.TotalPages,
            RowCount = result.RowCount,
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    [HttpPost]
    public async Task<ApiResult> ManualRewardPointAsync(CreateManualRewardPointInfoRequest request)
    {
        var result =
            await _rewardPointBll.CreateManualRewardPointInfoAsync(new CreateManualRewardPointInfoParams(request));

        return new ApiResult<TransIdModel>
        {
            Data = new TransIdModel(result.TransId),
            ErrorCode = (int)(result.Success ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }

    [HttpPost]
    public async Task<ApiResult> BatchManualRewardPointAsync(CreateManualRewardPointInfoBatchRequest request)
    {
        var result =
            await _rewardPointBll.CreateManualRewardPointInfoBatchAsync(new CreateManualRewardPointInfoBatchParams(request));

        return new ApiResult
        {
            ErrorCode = (int)(result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }
}