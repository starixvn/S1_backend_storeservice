using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Redemption;
using StoreService.Core.Service.Model.Report;

namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly RedemptionBLL _redemptionBll;
    
    public ReportController(IServiceProvider provider)
    {
        _redemptionBll = provider.GetRequiredService<RedemptionBLL>();
    }
    
    [HttpPost]
    public async Task<ApiResult> GetRedemptionReportAsync(GetRedemptionReportRequest request)
    {
        var result =
            await _redemptionBll.GetRedemptionReportAsync(new GetRedemptionListParams(request));
        
        return new ApiResult<GetRedemptionReportResult>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
}