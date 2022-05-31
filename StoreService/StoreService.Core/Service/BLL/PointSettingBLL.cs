using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.PointSetting;

namespace StoreService.Core.Service.BLL;

public class PointSettingBLL
{
    private PointSettingService _pointSettingService;
    private ILogger<PointSettingBLL> _logger;
    
    public PointSettingBLL(IServiceProvider provider)
    {
        _pointSettingService = provider.GetRequiredService<PointSettingService>();
        _logger = provider.GetRequiredService<ILogger<PointSettingBLL>>();
    }

    public Task<InactivePointExpirySetting> GetInactivePointExpirySettingAsync(
        GetInactivePointExpirySettingParams input)
        => _pointSettingService.GetInactivePointExpirySettingAsync(input);

    public async Task<bool> SetInactivePointExpirySettingAsync(
        SetInactivePointExpirySettingParams input)
    {
        var result = await _pointSettingService.SetInactivePointExpirySettingAsync(input);
        var success = result > 0;
        
        if (!success)
        {
            _logger.LogError($"SetInactivePointExpirySetting failed, effect rows: {result}");
        }
        
        return success;
    }
    
    public Task<IEnumerable<TurnoverPointSetting>> GetTurnoverPointSettingAsync(
        GetTurnoverPointSettingParams input)
        => _pointSettingService.GetTurnoverPointSettingAsync(input); 
    
    public Task<IEnumerable<SimpleTurnoverPointSetting>> GetTurnoverPointSettingListAsync(
        GetTurnoverPointSettingListParams input)
        => _pointSettingService.GetTurnoverPointSettingListAsync(input);
    
    public async Task<bool> SetTurnoverPointSettingAsync(
        SetTurnoverPointSettingParams input)
    {
        var result = await _pointSettingService.SetTurnoverPointSettingAsync(input);
        var success = result > 0;
        
        if (!success)
        {
            _logger.LogError($"SetTurnoverPointSetting failed, effect rows: {result}");
        }
        
        return success;
    }
    
    public Task<IEnumerable<DepositPointSetting>> GetDepositPointSettingAsync(
        GetDepositPointSettingParams input)
        => _pointSettingService.GetDepositPointSettingAsync(input);
    
    public async Task<bool> SetDepositPointSettingAsync(
        SetDepositPointSettingParams input)
    {
        var result = await _pointSettingService.SetDepositPointSettingAsync(input);
        var success = result > 0;
        
        if (!success)
        {
            _logger.LogError($"SetDepositPointSetting failed, effect rows: {result}");
        }
        
        return success;
    }
}