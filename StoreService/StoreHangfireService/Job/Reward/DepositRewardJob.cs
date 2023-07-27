using Hangfire;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Server;
using StoreHangfireService.Job.Interfaces;
using StoreHangfireService.Model.Options;
using StoreHangfireService.Service.Expired;
using StoreHangfireService.Service.Reward;
using System.ComponentModel;

namespace StoreHangfireService.Job.Reward;

[ManagementPage("DepositRewardJob", queue: "reward")]
public class DepositRewardJob : BaseJob, IScheduledJob
{
    private readonly DepositRewardService _depositRewardService;

    public DepositRewardJob(IServiceProvider provider) : base(provider)
    {
        _depositRewardService = provider.GetRequiredService<DepositRewardService>();
    }

    [DisplayName("{0}")]
    public async Task<object> ExecuteAsync(string jobName,
        JobOption jobOption,
        PerformContext context,
        IJobCancellationToken cancellationToken)
    {
        _logger.LogInformation($"{DateTime.Now:HH:mm:ss} || {jobName} running......");
        try
        {
            await _depositRewardService.RunJobAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, $"{DateTime.Now:HH:mm:ss} || {jobName} exception......");
            return false;
        }
        _logger.LogInformation($"{DateTime.Now:HH:mm:ss} || {jobName} End......");
        return true;
    }
}
