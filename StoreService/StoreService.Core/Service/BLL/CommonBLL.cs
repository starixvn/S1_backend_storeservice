using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.BaseData;

namespace StoreService.Core.Service.BLL;

public class CommonBLL
{
    private CommonService _commonService;
    private ILogger<CommonBLL> _logger;
    
    public CommonBLL(IServiceProvider provider)
    {
        _commonService = provider.GetRequiredService<CommonService>();
        _logger = provider.GetRequiredService<ILogger<CommonBLL>>();
    }

    public Task<IEnumerable<TpCategory>> GetTpCategoryAsync()
        => _commonService.GetTpCategoryAsync();
}