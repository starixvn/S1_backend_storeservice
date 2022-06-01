using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.BaseData;

namespace StoreService.Core.Service.BLL;

public class CommonBLL
{
    private CommonService _commonService;
    private CacheHelper _cacheHelper;
    private ILogger<CommonBLL> _logger;

    public CommonBLL(IServiceProvider provider)
    {
        _commonService = provider.GetRequiredService<CommonService>();
        _cacheHelper = provider.GetRequiredService<CacheHelper>();
        _logger = provider.GetRequiredService<ILogger<CommonBLL>>();
    }

    public Task<IEnumerable<TpCategory>> GetTpCategoryListAsync(GetTpCategoryListParams input)
    {
        var key = $"TpCategory_{input.SiteId}";

        return _cacheHelper.TryGetOrCreate(key,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            i => GetTpCategoryListAsync(input.SiteId));
    }
    
    public async Task<IEnumerable<TpGameType>> GetTpGameTypeListAsync(GetTpGameTypeListParams input)
    {
        var list = await GetTpGameTypeListAsync(input.SiteId);
        var result = list
            .Where(x => x.TpCategoryId == input.TpCategoryId)
            .Select(x => new TpGameType(x));
        
        return result;
    }

    public Task<IEnumerable<VipLevel>> GetVipLevelListAsync(GetVipLevelListParams input)
    {
        var key = $"VipLevel_{input.SiteId}";

        return _cacheHelper.TryGetOrCreate(key,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            i => _commonService.GetVipLevelListAsync(input));
    }

    #region Private Methods
    
    private Task<IEnumerable<TpInfo>> GetTpListAsync(int siteId)
    {
        var key = $"TpList_{siteId}";

        return _cacheHelper.TryGetOrCreate(key,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            i => _commonService.GetTpListAsync(siteId));
    }
    
    private async Task<IEnumerable<TpCategory>> GetTpCategoryListAsync(int siteId)
    {
        var tpList = await GetTpListAsync(siteId);
        var tpGameTypeList = await GetTpGameTypeListAsync(siteId);
        var result = tpGameTypeList
            .Where(x => tpList.Any(y => y.TpId == x.TpId))
            .Select(x => new TpCategory
            {
                Id = x.TpCategoryId,
                Name = x.TpCategoryName
            })
            .Distinct(new TpCategoryComparer())
            .OrderBy(x => x.Id);

        return result;
    }
    
    private Task<IEnumerable<TpGameTypeInfo>> GetTpGameTypeListAsync(int siteId)
    {
        var key = $"TpGameTypeList_{siteId}";

        return _cacheHelper.TryGetOrCreate(key,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            i => _commonService.GetTpGameTypeListAsync(siteId));
    }

    #endregion
}