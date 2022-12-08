using Newtonsoft.Json;
using StoreHangfireService.Job;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;
using StoreService.Core.Service.Model.PointSetting;

namespace StoreHangfireService.Service.Expired;

public class PointExpiryService
{
    private readonly PointSettingBLL _pointSettingbll;
    private readonly RewardPointBLL _rewardPointbll;
    private readonly MemberBLL _memberBll;
    private readonly ILogger<BaseJob> _logger;

    public PointExpiryService(IServiceProvider provider)
    {
        _pointSettingbll = provider.GetRequiredService<PointSettingBLL>();
        _rewardPointbll = provider.GetRequiredService<RewardPointBLL>();
        _memberBll = provider.GetRequiredService<MemberBLL>();
        _logger = provider.GetRequiredService<ILogger<BaseJob>>();
    }

    public async Task RunJobAsync()
    {
        // STEP1: 取得歸零膏設定
        var settings = await GetAllInactivePointExpirySettings();

        if (!settings.Any())
        {
            _logger.LogInformation("Inactive point expiry setting has no data.");
            return;
        }

        var enableSettings = settings.Where(x => x.Status == (int)GeneralStatusEnum.Enabled);

        if (!enableSettings.Any())
        {
            _logger.LogInformation("Has no enabled inactive point expiry settings.");
            return;
        }

        // STEP2: 取得非活躍用戶
        var inactiveMemberList = new List<CustomerInfo>();

        foreach (var setting in enableSettings)
            inactiveMemberList.AddRange(await GetInactiveMemberList(setting.SiteId, setting.Days));

        if (!inactiveMemberList.Any())
        {
            _logger.LogInformation("Has no inactive members.");
            return;
        }

        // STEP3: 點數歸零膏
        if (!await SetPointExpiry(inactiveMemberList))
            _logger.LogInformation("SetPointExpiry Failed." + JsonConvert.SerializeObject(inactiveMemberList));

        await Task.CompletedTask;
    }

    private async Task<IEnumerable<InactivePointExpirySetting>> GetAllInactivePointExpirySettings()
        => await _pointSettingbll.GetAllInactivePointExpirySettingsAsync();

    private async Task<IEnumerable<CustomerInfo>> GetInactiveMemberList(int siteId, int days)
        => await _memberBll.GetInactiveMemberListAsync(
                new GetInactiveMemberListParams
                {
                    SiteId = siteId,
                    Days = days
                });

    private async Task<bool> SetPointExpiry(List<CustomerInfo> inactiveMemberList)
    => await _rewardPointbll.SetPointExpiryAsync(inactiveMemberList);

}
