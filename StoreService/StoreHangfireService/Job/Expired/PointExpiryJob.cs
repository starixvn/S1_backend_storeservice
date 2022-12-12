using Hangfire;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Server;
using StoreHangfireService.Job.Interfaces;
using StoreHangfireService.Model.Options;
using StoreHangfireService.Service.Expired;
using System.ComponentModel;

namespace StoreHangfireService.Job.Expired;

[ManagementPage("PointExpiryJob", queue: "expired")]
public class PointExpiryJob : BaseJob, IScheduledJob
{
    private readonly PointExpiryService _pointExpiryService;

    public PointExpiryJob(IServiceProvider provider) : base(provider)
    {
        _pointExpiryService = provider.GetRequiredService<PointExpiryService>();
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
            await _pointExpiryService.RunJobAsync();
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
