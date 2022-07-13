using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Redemption;
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
    
    public async Task<PagerResultModel<GetDepositPointReportResult>> GetDepositPointReportAsync(GetDepositPointReportParams input)
    {
        var list = await _reportService.GetDepositPointReportAsync(input);

        return list;
    }
}