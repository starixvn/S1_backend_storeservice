using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
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

    public async Task<CustPointSummary> GetCustCoinAsync(GetMyCoinParams input)
    {
        var result = new List<RewardPointSummary>();
        var redemptionSummary = await GetRedemptionPointSummaryAsync(input);
        result.Add(redemptionSummary.Summary);
        result.Add(await GetTurnoverPointSummaryAsync(input));
        result.AddRange(await GetRewardPointSummaryAsync(input));

        var custRewardPoint = _memberService.GetCustRewardPointAsync(input) ?? new CustPointSummary() ;
        custRewardPoint.PointLocked = redemptionSummary.LockedPoint;
        custRewardPoint.Detail = result.OrderBy(x => x.PointType).ToList();
        return custRewardPoint;
    }

    private async Task<(RewardPointSummary Summary, decimal LockedPoint)> GetRedemptionPointSummaryAsync(GetMyCoinParams input)
    {
        var result = await _memberService.GetRedemptionPointSummaryAsync(input);
        var summary = result.Where(x => x.Status != RedemptionEnum.Rejected).Sum(x => x.Summary);
        var lockedPoint = result.Where(x => x.Status != RedemptionEnum.Rejected && x.Status != RedemptionEnum.Approved).Sum(x => x.Summary);
        return (new RewardPointSummary()
        {
            PointType = PointTypeEnum.Redemption,
            MinusSummary = summary,
            TotalSummary = summary
        }, lockedPoint);
    }
    private async Task<RewardPointSummary> GetTurnoverPointSummaryAsync(GetMyCoinParams input)
    {
        var result = await _memberService.GetTurnoverPointSummaryAsync(input);
        var summary = result.Sum(x => x.Summary);
        return new RewardPointSummary()
        {
            PointType = PointTypeEnum.Turnover,
            PlusSummary = summary,
            TotalSummary = summary,
            DetailRewardCoin = result.ToDictionary(t => t.CategoryName, t => t.Summary)
        };
    }
    private async Task<IEnumerable<RewardPointSummary>> GetRewardPointSummaryAsync(GetMyCoinParams input)
    {
        var result = await _memberService.GetRewardPointSummaryAsync(input);
        var list = result.Where(x => !new PointTypeEnum[] { PointTypeEnum.Redemption, PointTypeEnum.Turnover }.Contains(x.PointType)).ToList();
        #region ��0

        if (!list.Any(x => x.PointType == PointTypeEnum.Deposit))
            list.Add(new RewardPointSummary() { PointType = PointTypeEnum.Deposit });

        if (!list.Any(x => x.PointType == PointTypeEnum.Manual))
            list.Add(new RewardPointSummary() { PointType = PointTypeEnum.Manual });

        if (!list.Any(x => x.PointType == PointTypeEnum.System))
            list.Add(new RewardPointSummary() { PointType = PointTypeEnum.System });

        #endregion
        return list;
    }
}