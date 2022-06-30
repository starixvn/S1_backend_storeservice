using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Redemption;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class RedemptionController : ControllerBase
{
    private readonly RedemptionBLL _redemptionBll;
    
    public RedemptionController(IServiceProvider provider)
    {
        _redemptionBll = provider.GetRequiredService<RedemptionBLL>();
    }
    
    #region Redemption

    [HttpPost]
    public async Task<ApiResult> CreateRedemptionAsync(CreateRedemptionRequest request)
    {
        var result =
            await _redemptionBll.CreateRedemptionAsync(new CreateRedemptionParams(request));

        return new ApiResult
        {
            ErrorCode = (int) (result ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed)
        };
    }

    #endregion
}