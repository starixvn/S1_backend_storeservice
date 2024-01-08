using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Report;

namespace StoreService.Core.Service.BLL;

public class ReportBLL
{
    private readonly ReportService _reportService;
    private readonly ILogger<RedemptionBLL> _logger;

    public ReportBLL(IServiceProvider provider)
    {
        _reportService = provider.GetRequiredService<ReportService>();
        _logger = provider.GetRequiredService<ILogger<RedemptionBLL>>();
    }

    #region DepositPoint

    public async Task<DepositPointReportResult> GetDepositPointReportAsync(GetDepositPointReportParams input)
    {
        var list = await _reportService.GetDepositPointReportAsync(input);
        var overview = await _reportService.GetDepositPointOverviewAsync(input);

        var result = new DepositPointReportResult
        {
            List = new PagerResponseModel<GetDepositPointReportResult>
            {
                Datas = list.Datas,
                PageNumber = input.PageNumber,
                RowCount = list.RowCount,
                TotalPages = list.TotalPages
            },
            Overview = overview
        };

        return result;
    }

    public async Task<ExportDepositPointReportResult> ExportDepositPointReportAsync(ExportDepositPointReportParams input)
    {
        var list = await _reportService.ExportDepositPointReportAsync(input);
        var overview = await _reportService.GetDepositPointOverviewAsync(new GetDepositPointReportParams(input));

        var result = new ExportDepositPointReportResult
        {
            List = list,
            Overview = overview
        };

        return result;
    }

    #endregion

    #region AdjustmentPoint

    public async Task<AdjustmentPointReportResult> GetAdjustmentPointReportAsync(GetAdjustmentPointReportParams input)
    {
        var list = await _reportService.GetAdjustmentPointReportAsync(input);
        var overview = await _reportService.GetAdjustmentPointOverviewAsync(input);

        var result = new AdjustmentPointReportResult
        {
            List = new PagerResponseModel<GetAdjustmentPointReportResult>
            {
                Datas = list.Datas,
                PageNumber = input.PageNumber,
                RowCount = list.RowCount,
                TotalPages = list.TotalPages
            },
            Overview = overview
        };

        return result;
    }

    public async Task<ExportAdjustmentPointReportResult> ExportAdjustmentPointReportAsync(ExportAdjustmentPointReportParams input)
    {
        var list = await _reportService.ExportAdjustmentPointReportAsync(input);
        var overview = await _reportService.GetAdjustmentPointOverviewAsync(new GetAdjustmentPointReportParams(input));

        var result = new ExportAdjustmentPointReportResult
        {
            List = list,
            Overview = overview
        };

        return result;
    }

    #endregion

    #region TurnoverPoint

    public async Task<TurnoverPointReportResult> GetTurnoverPointReportAsync(GetTurnoverPointReportParams input)
    {
        var list = await _reportService.GetTurnoverPointReportAsync(input);
        var overview = await _reportService.GetTurnoverPointOverviewAsync(input);

        var result = new TurnoverPointReportResult
        {
            List = new PagerResponseModel<GetTurnoverPointReportResult>
            {
                Datas = list.Datas,
                PageNumber = input.PageNumber,
                RowCount = list.RowCount,
                TotalPages = list.TotalPages
            },
            Overview = overview
        };

        return result;
    }

    public async Task<ExportTurnoverPointReportResult> ExportTurnoverPointReportAsync(ExportTurnoverPointReportParams input)
    {
        var list = await _reportService.ExportTurnoverPointReportAsync(input);
        var overview = await _reportService.GetTurnoverPointOverviewAsync(new GetTurnoverPointReportParams(input));

        var result = new ExportTurnoverPointReportResult
        {
            List = list,
            Overview = overview
        };

        return result;
    }

    #endregion

}