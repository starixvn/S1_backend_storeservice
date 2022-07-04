using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreServiceWorker.Workers
{
    public class TurnoverRewardPointWorker : BackgroundService
    {
        private readonly RewardPointService _rewardPointService;
        private readonly ILogger<TurnoverRewardPointWorker> _logger;
        private Timer _timer;

        public TurnoverRewardPointWorker(IServiceProvider provider)
        {
            _rewardPointService = provider.GetRequiredService<RewardPointService>();
            _logger = provider.GetRequiredService<ILogger<TurnoverRewardPointWorker>>();
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
}
