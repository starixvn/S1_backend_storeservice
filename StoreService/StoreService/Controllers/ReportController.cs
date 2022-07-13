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
        
        
        return new ApiResult<DepositPointReportResult>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
    
    [HttpPost]
    public async Task<ApiResult> ExportDepositPointReportAsync(ExportDepositPointReportRequest request)
    {
        var result =
            await _reportBLL.ExportDepositPointReportAsync(new ExportDepositPointReportParams(request));
        
        return new ApiResult<ExportDepositPointReportResult>
        {
            Data = result,
            ErrorCode = (int) ErrorCodeEnum.Success
        };
    }
}