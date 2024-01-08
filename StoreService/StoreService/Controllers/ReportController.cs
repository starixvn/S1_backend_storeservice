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

    #region DepositPoint

    [HttpPost]
    public async Task<ApiResult> GetDepositPointReportAsync(GetDepositPointReportRequest request)
    {
        var result =
            await _reportBLL.GetDepositPointReportAsync(new GetDepositPointReportParams(request));


        return new ApiResult<DepositPointReportResult>
        {
            Data = result,
            ErrorCode = (int)ErrorCodeEnum.Success
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
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    #endregion

    #region AdjustmentPoint

    [HttpPost]
    public async Task<ApiResult> GetAdjustmentPointReportAsync(GetAdjustmentPointReportRequest request)
    {
        var result =
            await _reportBLL.GetAdjustmentPointReportAsync(new GetAdjustmentPointReportParams(request));


        return new ApiResult<AdjustmentPointReportResult>
        {
            Data = result,
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    [HttpPost]
    public async Task<ApiResult> ExportAdjustmentPointReportAsync(ExportAdjustmentPointReportRequest request)
    {
        var result =
            await _reportBLL.ExportAdjustmentPointReportAsync(new ExportAdjustmentPointReportParams(request));

        return new ApiResult<ExportAdjustmentPointReportResult>
        {
            Data = result,
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    #endregion

    #region TurnoverPoint

    [HttpPost]
    public async Task<ApiResult> GetTurnoverPointReportAsync(GetTurnoverPointReportRequest request)
    {
        var result =
            await _reportBLL.GetTurnoverPointReportAsync(new GetTurnoverPointReportParams(request));


        return new ApiResult<TurnoverPointReportResult>
        {
            Data = result,
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    [HttpPost]
    public async Task<ApiResult> ExportTurnoverPointReportAsync(ExportTurnoverPointReportRequest request)
    {
        var result =
            await _reportBLL.ExportTurnoverPointReportAsync(new ExportTurnoverPointReportParams(request));

        return new ApiResult<ExportTurnoverPointReportResult>
        {
            Data = result,
            ErrorCode = (int)ErrorCodeEnum.Success
        };
    }

    #endregion
}