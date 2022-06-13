using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Extensions;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;

namespace StoreService.Core.Service.BLL;

public class MemberBLL
{
    private MemberService _memberService;
    private ILogger<MemberBLL> _logger;
    private readonly string _prefix;
    
    public MemberBLL(IServiceProvider provider)
    {
        _memberService = provider.GetRequiredService<MemberService>();
        _logger = provider.GetRequiredService<ILogger<MemberBLL>>();
        var env = provider.GetRequiredService<IWebHostEnvironment>();
        _prefix = env.EnvironmentName.GetEnvPrefix("P");
    }

    public Task<PagerResultModel<MemberRewardPointInfo>> GetMemberRewardPointListAsync(GetListParams input)
        => _memberService.GetMemberRewardPointListAsync(input);
    
    public Task<PagerResultModel<MemberRewardPointLog>> GetMemberRewardPointLogAsync(GetMemberRewardPointLogParams input)
        => _memberService.GetMemberRewardPointLogAsync(input);

    public async Task<(bool Success, string TransId)> CreateManualRewardPointInfoAsync(CreateManualRewardPointInfoParams input)
    {
        var result = await _memberService.CreateManualRewardPointInfoAsync(input, _prefix);
        var success = result.ResultCode == (int) ErrorCodeEnum.Success;

        if (!success)
        {
            _logger.LogError($"Insert ManualRewardPointInfo Failed, Result Code: ({result.ResultCode})");
        }
        
        return (success, result.TransId);
    }
    
    public async Task<bool> CreateManualRewardPointInfoBatchAsync(CreateManualRewardPointInfoBatchParams input)
    {
        var result = await _memberService.CreateManualRewardPointInfoBatchAsync(input, _prefix);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError($"Batch ManualRewardPointInfo Failed, Result: ({result})");
        }
        
        return success;
    }
}