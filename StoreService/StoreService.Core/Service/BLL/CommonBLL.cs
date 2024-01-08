using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model;

namespace StoreService.Core.Service.BLL;

public class CommonBLL
{
    private CommonService _commonService;
    private CacheHelper _cacheHelper;

    public CommonBLL(IServiceProvider provider)
    {
        _commonService = provider.GetRequiredService<CommonService>();
        _cacheHelper = provider.GetRequiredService<CacheHelper>();
    }

    public Task<IEnumerable<CurrencyInfo>> GetCurrencyInfoAsync()
    {
        return _cacheHelper.TryGetOrCreate("CurrencyInfoList",
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            i => _commonService.GetCurrencyInfoAsync());
    }
}