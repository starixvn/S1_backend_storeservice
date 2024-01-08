using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.PointSetting;

namespace StoreService.Core.Service.BLL;

public class PointSettingBLL
{
    private readonly PointSettingService _pointSettingService;
    private readonly ILogger<PointSettingBLL> _logger;

    public PointSettingBLL(IServiceProvider provider)
    {
        _pointSettingService = provider.GetRequiredService<PointSettingService>();
        _logger = provider.GetRequiredService<ILogger<PointSettingBLL>>();
    }

    public async Task<InactivePointExpirySetting> GetInactivePointExpirySettingAsync(
        GetInactivePointExpirySettingParams input)
        => (await _pointSettingService.GetInactivePointExpirySettingAsync(input)).FirstOrDefault();

    public Task<IEnumerable<InactivePointExpirySetting>> GetAllInactivePointExpirySettingsAsync()
        => _pointSettingService.GetInactivePointExpirySettingAsync(
            new GetInactivePointExpirySettingParams(-1)); // SiteId -1: all 

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

    public async Task<IEnumerable<TurnoverPointSettingResult>> GetTurnoverPointSettingListAsync(
        GetTurnoverPointSettingListParams input)
    {
        var list = await _pointSettingService.GetTurnoverPointSettingListAsync(input);

        var vipLevels = list
            .Select(x => new VipLevel(x.VipLevel, x.VipLevelName))
            .Distinct(new VipLevelComparer())
            .OrderBy(x => x.LevelId);

        var result =
            vipLevels.Select(x => new TurnoverPointSettingResult
            {
                VipLevel = x.LevelId,
                VipLevelName = x.LevelName,
                TpCategoryTurnoverPointSetting = list
                    .Where(y => y.VipLevel == x.LevelId)
                    .Select(y => new TpCategoryTurnoverPointSetting
                    {
                        TpCategoryId = y.TpCategoryId,
                        Turnover = y.Turnover,
                        PointRate = y.PointRate,
                        PromotionStatus = y.PromotionStatus,
                        PromotionPointRate = y.PromotionPointRate,
                        PromotionStartTime = y.PromotionStartTime,
                        PromotionEndTime = y.PromotionEndTime
                    })
                    .OrderBy(y => y.TpCategoryId)
            });

        return result;
    }

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

    public Task<IEnumerable<TurnoverPointStatusSetting>> GetTurnoverPointStatusSettingAsync(
        GetTurnoverPointStatusSettingParams input)
        => _pointSettingService.GetTurnoverPointStatusSettingAsync(input);

    public async Task<bool> SetTurnoverPointStatusSettingAsync(
        SetTurnoverPointStatusSettingParams input)
    {
        var result = await _pointSettingService.SetTurnoverPointStatusSettingAsync(input);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError($"SetTurnoverPointStatusSetting failed, effect rows: {result}");
        }

        return success;
    }
}