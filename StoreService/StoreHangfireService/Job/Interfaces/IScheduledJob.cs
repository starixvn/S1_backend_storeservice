using Hangfire;
using Hangfire.Server;

using StoreHangfireService.Model.Options;

namespace StoreHangfireService.Job.Interfaces;

internal interface IScheduledJob
{
    Task<object> ExecuteAsync(string jobName, JobOption jobOption, PerformContext context, IJobCancellationToken cancellationToken);
}
