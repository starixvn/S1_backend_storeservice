using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Redemption;

namespace StoreService.Core.Service.BLL;

public class RedemptionBLL
{
    private readonly RedemptionService _redemptionService;
    private readonly ILogger<RedemptionBLL> _logger;

    public RedemptionBLL(IServiceProvider provider)
    {
        _redemptionService = provider.GetRequiredService<RedemptionService>();
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
   
    public async Task<int> CheckCustRedemptionLimitAsync(int siteId, int currencyId , int custId )
    {
        var result = await _redemptionService.CheckCustRedemptionLimitAsync(siteId,currencyId,custId);
        return result;
    }
    
    public async Task<PagerResultModel<GetRedemptionListResult>> GetRedemptionListAsync(GetRedemptionListRequest input)
    {
        var result = await _redemptionService.GetRedemptionListAsync(input);
        return result;
    }
    
    public async Task<IEnumerable<GetRedemptionListResult>> ExportRedemptionListAsync(ExportRedemptionListRequest input)
    {
        var result = await _redemptionService.ExportRedemptionListAsync(input);
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
    
    public async Task<bool> SetRedemptionPendingAsync(SetRedemptionInfoRequest input)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input);
        return result;
    }
    
    public async Task<bool> SetRedemptionDraftAsync(SetRedemptionInfoRequest input)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input);
        return result;
    }
    
    public async Task<bool> SetRedemptionApprovedAsync(SetRedemptionInfoRequest input)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input);
        return result;
    }
    
    public async Task<bool> SetRedemptionRejectedAsync(SetRedemptionInfoRequest input)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input);
        return result;
    }
    
    public async Task<bool> SetRedemptionOnHoldAsync(SetRedemptionInfoRequest input)
    {
        var result = await _redemptionService.SetRedemptionInfoAsync(input);
        return result;
    }
    
    public async Task<IEnumerable<GetRedemptionLog>> GetRedemptionLogAsync(GetRedemptionLogRequest input)
    {
        var result = await _redemptionService.GetRedemptionLogAsync(input);
        return result;
    }
}