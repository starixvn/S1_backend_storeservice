using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.DAL;

namespace StoreServiceWorker.Workers;

public class DepositRewardPointWorker : BackgroundService
{
    private readonly RewardPointService _rewardPointService;
    private readonly ILogger<DepositRewardPointWorker> _logger;
    private Timer _timer;

    public DepositRewardPointWorker(IServiceProvider provider)
    {
        _rewardPointService = provider.GetRequiredService<RewardPointService>();
        _logger = provider.GetRequiredService<ILogger<DepositRewardPointWorker>>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);

        return Task.CompletedTask;
    }

    private async void DoTaskAsync(object state)
    {
        try
        {
            var queue = await _rewardPointService.GetDepositRewardPointQueueAsync();

            if (!queue.Any())
            {
                _logger.LogInformation("No data.");
                return;
            }

            var groupQueue = queue.GroupBy(x => x.CurrencyId);
            foreach (var group in groupQueue)
            {
                //var depositInfos = group.Select(x => x.)
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
        }
        finally
        {
            _timer.Change(TimeSpan.FromMinutes(5), TimeSpan.Zero);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}