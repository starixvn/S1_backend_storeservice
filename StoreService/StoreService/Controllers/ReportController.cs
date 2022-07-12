using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Report;


namespace StoreService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly ReportBLL _reportBLL;

    public ReportController(IServiceProvider provider)
    {
        _reportBLL = provider.GetRequiredService<ReportBLL>();
    }
    
    [HttpPost]
    public async Task<ApiResult> GetDepositPointReportAsync(GetDepositPointReportRequest request)
    {
        var result =
            await _reportBLL.GetDepositPointReportAsync(new GetDepositPointReportParams(request));
        
        return new ApiResult<IEnumerable<GetDepositPointReportResult>>
        {
            Data = result.Datas,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
}