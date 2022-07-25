using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model;
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

    public async Task<MemberPointBalance> GetMemberPointBalanceAsync(int custId)
    {
        var result = await _memberService.GetMemberPointBalanceAsync(custId) 
            ?? new MemberPointBalance
            {
                PointBalance = 0
            };

        return result;
    }

    public Task<PagerResultModel<MemberRewardPointInfo>> GetMemberRewardPointListAsync(GetListParams input)
        => _memberService.GetMemberRewardPointListAsync(input);

    public Task<PagerResultModel<MemberRewardPointLog>> GetMemberRewardPointLogAsync(GetMemberRewardPointLogParams input)
        => _memberService.GetMemberRewardPointLogAsync(input);

    public Task<IEnumerable<CustomerInfo>> GetInactiveMemberListAsync(GetInactiveMemberListParams input)
        => _memberService.GetInactiveMemberListAsync(input);

    public async Task<IEnumerable<int>> GetCustFavoriteGiftAsync(int custId)
    {
        var strFavorite = await _memberService.GetCustFavoriteGiftAsync(custId);
        var result = string.IsNullOrWhiteSpace(strFavorite) ? new List<int>() : JsonConvert.DeserializeObject<IEnumerable<int>>(strFavorite);

        return result;
    }

    public async Task<bool> SetCustFavoriteGiftAsync(SetFavoriteParams input)
    {
        var result = await _memberService.SetCustFavoriteGiftAsync(input);

        return result > 0;
    }
}