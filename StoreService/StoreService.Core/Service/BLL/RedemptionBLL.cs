using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Redemption;

namespace StoreService.Core.Service.BLL;

public class RedemptionBLL
{
    private readonly RedemptionService _redemptionService;
    private readonly CommonBLL _commonBll;
    private readonly ILogger<RedemptionBLL> _logger;

    public RedemptionBLL(IServiceProvider provider)
    {
        _redemptionService = provider.GetRequiredService<RedemptionService>();
        _commonBll = provider.GetRequiredService<CommonBLL>();
        _logger = provider.GetRequiredService<ILogger<RedemptionBLL>>();
    }

    public async Task<bool> CreateRedemptionAsync(CreateRedemptionParams input)
    {
        var success = await _redemptionService.CreateRedemptionAsync(input);

        if (!success)
        {
            _logger.LogError("Create RedemptionInfo Failed");
        }

        return success;
    }

    public async Task<int> CheckCustRedemptionLimitAsync(int siteId, int currencyId, int custId, int point)
    {
        var result = await _redemptionService.CheckCustRedemptionLimitAsync(siteId, currencyId, custId, point);
        return result;
    }

    public async Task<RedemptionListResult> GetRedemptionListAsync(GetRedemptionListParams input)
    {
        var list = await _redemptionService.GetRedemptionListAsync(input);
        var overview = await GetRedemptionOverviewAsync(input);

        var result = new RedemptionListResult
        {
            List = new PagerResponseModel<GetRedemptionListResult>
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

    public async Task<ExportRedemptionListResult> ExportRedemptionListAsync(ExportRedemptionListParams input)
    {
        var list = await _redemptionService.ExportRedemptionListAsync(input);
        var overview =
            await GetRedemptionOverviewAsync(new GetRedemptionListParams(input, input.CurrencyIds));

        var result = new ExportRedemptionListResult
        {
            List = list,
            Overview = overview
        };

        return result;
    }

    public async Task<GetRedemptionDetail> GetRedemptionDetailAsync(GetRedemptionDetailRequest input)
    {
        var result = await _redemptionService.GetRedemptionDetailAsync(input);
        return result;
    }

    public async Task<GetRedemptionByTransIdResult> GetRedemptionByTransIdAsync(string transId)
    {
        var result = await _redemptionService.GetRedemptionByTransIdAsync(transId);
        return result;
    }

    public async Task<bool> SetRedemptionPendingAsync(SetRedemptionInfoRequest input, GetRedemptionByTransIdResult info)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input, info);
        return result;
    }

    public async Task<bool> SetRedemptionDraftAsync(SetRedemptionInfoRequest input, GetRedemptionByTransIdResult info)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input, info);
        return result;
    }

    public async Task<bool> SetRedemptionApprovedAsync(SetRedemptionInfoRequest input, GetRedemptionByTransIdResult info)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input, info);
        return result;
    }

    public async Task<bool> SetRedemptionRejectedAsync(SetRedemptionInfoRequest input, GetRedemptionByTransIdResult info)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input, info);
        return result;
    }

    public async Task<bool> SetRedemptionOnHoldAsync(SetRedemptionInfoRequest input, GetRedemptionByTransIdResult info)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input, info);
        return result;
    }

    public async Task<IEnumerable<GetRedemptionLog>> GetRedemptionLogAsync(GetRedemptionLogRequest input)
    {
        var result = await _redemptionService.GetRedemptionLogAsync(input);
        return result;
    }

    public async Task<MyPurchesResult> GetMyPurchesAsync(GetMyPurchesParams input)
    {
        var list = await _redemptionService.GetMyPurchesAsync(input);
        var result = new MyPurchesResult
        {
            List = new PagerResponseModel<GetMyPurchesResult>
            {
                Datas = list.Datas,
                PageNumber = input.PageNumber,
                RowCount = list.RowCount,
                TotalPages = list.TotalPages
            }
        };

        return result;
    }


    #region Private Methods

    private async Task<IEnumerable<GetRedemptionOverviewResult>> GetRedemptionOverviewAsync(
        GetRedemptionListParams input)
    {
        var overview = await _redemptionService.GetRedemptionOverviewAsync(input);
        var currencyList = (await _commonBll.GetCurrencyInfoAsync())
            .Where(x => input.CurrencyIds.Contains(x.CurrencyId));

        var result = currencyList
            .GroupJoin(overview,
                x => x.CurrencyId,
                y => y.CurrencyId,
                (x, y) => new GetRedemptionOverviewResult
                {
                    CurrencyId = x.CurrencyId,
                    IsoCode = x.IsoCode,
                    TotalCost = y.FirstOrDefault()?.TotalCost ?? 0m,
                    TotalPoint = y.FirstOrDefault()?.TotalPoint ?? 0m,
                    TotalRecord = y.FirstOrDefault()?.TotalRecord ?? 0
                });

        return result;
    }

    #endregion
}