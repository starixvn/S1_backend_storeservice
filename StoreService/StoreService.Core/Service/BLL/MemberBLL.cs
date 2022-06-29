using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Member;

namespace StoreService.Core.Service.BLL;

public class MemberBLL
{
    private MemberService _memberService;
    private readonly ILogger<MemberBLL> _logger;
    
    public MemberBLL(IServiceProvider provider)
    {
        _memberService = provider.GetRequiredService<MemberService>();
        _logger = provider.GetRequiredService<ILogger<MemberBLL>>();
    }

    public Task<PagerResultModel<MemberRewardPointInfo>> GetMemberRewardPointListAsync(GetListParams input)
        => _memberService.GetMemberRewardPointListAsync(input);
    
    public Task<PagerResultModel<MemberRewardPointLog>> GetMemberRewardPointLogAsync(GetMemberRewardPointLogParams input)
        => _memberService.GetMemberRewardPointLogAsync(input);
}