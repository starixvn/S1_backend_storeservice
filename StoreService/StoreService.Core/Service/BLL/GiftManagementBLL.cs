using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.GiftManagement;

namespace StoreService.Core.Service.BLL;

public class GiftManagementBLL
{
    private GiftManagementService _giftManagementService;
    private CacheHelper _cacheHelper;
    private ILogger<GiftManagementBLL> _logger;
    
    public GiftManagementBLL(IServiceProvider provider)
    {
        _giftManagementService = provider.GetRequiredService<GiftManagementService>();
        _cacheHelper = provider.GetRequiredService<CacheHelper>();
        _logger = provider.GetRequiredService<ILogger<GiftManagementBLL>>();
    }

    public Task<PagerResultModel<Gift>> GetGiftListAsync(GetListParams input)
        => _giftManagementService.GetGiftListAsync(input);

    public async Task<IEnumerable<GiftTag>> GetGiftTagsAsync(GetTagsParams input)
    {
        var key = $"GiftTags_{input.CurrencyId}_{input.SiteId}";
        
        return await _cacheHelper.TryGetOrCreate(key,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            i => _giftManagementService.GetTagsAsync(input));
    }

    public async Task<IEnumerable<GiftCategory>> GetGiftCategoriesAsync(GetCategoriesParams input)
    {
        var key = $"GiftCategories_{input.CurrencyId}_{input.SiteId}";
        
        return await _cacheHelper.TryGetOrCreate(key,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            i => _giftManagementService.GetCategoriesAsync(input));
    }
    
    public async Task<bool> CreateGiftAsync(CreateGiftParams input)
    {
        var resultCode = await _giftManagementService.CreateGiftAsync(input);
        var success = resultCode == (int) ErrorCodeEnum.Success;

        if (!success)
        {
            _logger.LogError($"Insert Gift Failed, Result Code: ({resultCode})");
        }
        
        return success;
    }
    
    public async Task<bool> UpdateGiftAsync(UpdateGiftParams input)
    {
        var resultCode = await _giftManagementService.UpdateGiftAsync(input);
        var success = resultCode == (int) ErrorCodeEnum.Success;

        if (!success)
        {
            _logger.LogError($"Update Gift Failed, Result Code: ({resultCode})");
        }
        
        return success;
    }
    
    public async Task<bool> CreateCategoryAsync(CreateCategoryParams input)
    {
        var result = await _giftManagementService.CreateCategoryAsync(input);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError("Insert Gift Category Failed");
        }
        
        return success;
    }
    
    public async Task<bool> UpdateCategoryAsync(UpdateCategoryParams input)
    {
        var result = await _giftManagementService.UpdateCategoryAsync(input);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError("Update Gift Category Failed");
        }
        
        return success;
    }
    
    public async Task<bool> CreateTagAsync(CreateTagParams input)
    {
        var result = await _giftManagementService.CreateTagAsync(input);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError("Insert Gift Tag Failed");
        }
        
        return success;
    }
    
    public async Task<bool> UpdateTagAsync(UpdateTagParams input)
    {
        var result = await _giftManagementService.UpdateTagAsync(input);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError("Update Gift Tag Failed");
        }
        
        return success;
    }

    public Task<RedemptionLimit> GetRedemptionLimitAsync(GetRedemptionLimitParams input)
        => _giftManagementService.GetRedemptionLimitAsync(input);
    
    public async Task<bool> SetRedemptionLimitAsync(SetRedemptionLimitParams input)
    {
        var result = await _giftManagementService.SetRedemptionLimitAsync(input);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError("Set Redemption Limit Failed");
        }
        
        return success;
    }
}