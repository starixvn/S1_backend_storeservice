using Hangfire;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Server;
using StoreHangfireService.Job.Interfaces;
using StoreHangfireService.Model.Options;
using StoreHangfireService.Service.Reward;
using System.ComponentModel;

namespace StoreHangfireService.Job.Reward;

//[ManagementPage("TurnoverRewardJob", queue: "reward")]
//public class TurnoverRewardJob : BaseJob, IScheduledJob
//{
//    private readonly TurnoverRewardService _turnoverRewardService;

//    public TurnoverRewardJob(IServiceProvider provider) : base(provider)
//    {
//        _turnoverRewardService = provider.GetRequiredService<TurnoverRewardService>();
//    }

//    [DisplayName("{0}")]
//    public async Task<object> ExecuteAsync(string jobName,
//        JobOption jobOption,
//        PerformContext context,
//        IJobCancellationToken cancellationToken)
//    {
//        _logger.LogInformation($"{DateTime.Now:HH:mm:ss} || {jobName} running......");
//        try
//        {
//            await _turnoverRewardService.RunJobAsync();
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, ex.Message, $"{DateTime.Now:HH:mm:ss} || {jobName} exception......");
//            return false;
//        }
//        _logger.LogInformation($"{DateTime.Now:HH:mm:ss} || {jobName} End......");
//        return true;
//    }
//}
