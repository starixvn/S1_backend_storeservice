using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Extensions;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreService.Core.Service.BLL;

public class RewardPointBLL
{
    private readonly string _prefix;
    private readonly RewardPointService _rewardPointService;
    private readonly ILogger<RewardPointBLL> _logger;
    private readonly string _connectionString;

    public RewardPointBLL(IServiceProvider provider)
    {
        _rewardPointService = provider.GetRequiredService<RewardPointService>();
        _logger = provider.GetRequiredService<ILogger<RewardPointBLL>>();
        var env = provider.GetRequiredService<IWebHostEnvironment>();
        _prefix = env.EnvironmentName.GetEnvPrefix("P");
    }

    public async Task<(bool Success, string TransId)> CreateManualRewardPointInfoAsync(
        CreateManualRewardPointInfoParams input)
    {
        var result = await _rewardPointService.CreateManualRewardPointInfoAsync(input, _prefix);
        var success = result.ResultCode == (int) ErrorCodeEnum.Success;

        if (!success)
        {
            _logger.LogError($"Insert ManualRewardPointInfo Failed, Result Code: ({result.ResultCode})");
        }

        return (success, result.TransId);
    }

    public async Task<bool> CreateManualRewardPointInfoBatchAsync(CreateManualRewardPointInfoBatchParams input)
    {
        var result = await _rewardPointService.CreateManualRewardPointInfoBatchAsync(input, _prefix);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError($"CreateManualRewardPointInfoBatch Failed, Result: ({result})");
        }

        return success;
    }

    public Task<IEnumerable<DepositPointSetting>> GetAllDepositPointSettingAsync()
        => _rewardPointService.GetAllDepositPointSettingAsync();

    public Task<IEnumerable<DepositRewardPointQueue>> GetDepositRewardPointQueueAsync()
        => _rewardPointService.GetDepositRewardPointQueueAsync();

    public async Task<bool> DepositBatchRewardPointAsync(DepositBatchRewardPointParams input)
    {
        var result = await _rewardPointService.DepositBatchRewardPointAsync(input);
        var success = result > 0;

        if (!success)
        {
            _logger.LogError($"DepositBatchRewardPoint Failed, Result: ({result})");
        }

        return success;
    }
}