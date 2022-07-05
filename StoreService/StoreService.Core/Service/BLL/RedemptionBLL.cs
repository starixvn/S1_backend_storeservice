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
    
    public async Task<PagerResultModel<GetRedemptionListResult>> GetRedemptionListAsync(GetRedemptionListRequest input)
    {
        var result = await _redemptionService.GetRedemptionListAsync(input);
        return result;
    }
    
    public async Task<GetRedemptionDetail> GetRedemptionDetailAsync(GetRedemptionDetailRequest input)
    {
        var result = await _redemptionService.GetRedemptionDetailAsync(input);
        return result;
    }
}