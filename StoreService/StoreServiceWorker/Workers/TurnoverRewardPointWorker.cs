using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreService.Core.Service.BLL;
using System.Diagnostics;

namespace StoreServiceWorker.Workers
{
    public class TurnoverRewardPointWorker : BackgroundService
    {
        private readonly TurnoverWorkerBLL _turnoverWorkerBLL;
        private readonly ILogger<TurnoverRewardPointWorker> _logger;
        private Timer _timer;

        public TurnoverRewardPointWorker(IServiceProvider provider)
        {
            _turnoverWorkerBLL = provider.GetRequiredService<TurnoverWorkerBLL>();
            _logger = provider.GetRequiredService<ILogger<TurnoverRewardPointWorker>>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);

            return Task.CompletedTask;
        }

        private async void DoTaskAsync(object state)
        {
            var isError = false;
            try
            {
                _logger.LogInformation("TurnoverWorker：Verify Task Start.");

                var doTaskList = await _turnoverWorkerBLL.VerifyTaskAsync();

                _logger.LogInformation($"TurnoverWorker : Verify Task End. Count = {doTaskList.Count()}");

                foreach (var task in doTaskList)
                {
                    _logger.LogInformation($"TurnoverWorker : Do Task Start. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.");
                    var sw = Stopwatch.StartNew();

                    var pointSetting = await _turnoverWorkerBLL.GetSettingAsync(task.SiteId, task.CurrencyId, task.TimeZone);

                    var custList = await _turnoverWorkerBLL.GetCustListAsync(task.SiteId, task.CurrencyId, task.TimeZone);

                    if (!await _turnoverWorkerBLL.CaculatePointAsync(task, custList, pointSetting))
                        throw new Exception();

                    if (!await _turnoverWorkerBLL.UpdateTaskSettledTImeASync(task.SiteId, task.CurrencyId))
                    {
                        _logger.LogError($"TurnoverWorker : Update Task SettleTime Fail. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.");
                        throw new Exception();
                    }

                    _logger.LogInformation($"TurnoverWorker : Do Task End. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.\n"
                    + $"Finish Time (m:s.ms): {Math.Round(sw.Elapsed.TotalMinutes)}:{Math.Round(sw.Elapsed.TotalMinutes)}.{Math.Round(sw.Elapsed.TotalMilliseconds)}.");
                }
            }
            catch (Exception ex)
            {
                isError = true;
                _logger.LogError(ex, "TurnoverWorker Exception");
            }
            finally
            {
                var dueTimeSpan = isError ? TimeSpan.FromMinutes(10) : TimeSpan.FromMinutes(60);

                _logger.LogInformation($"TurnoverWorker Thread End. isError={isError}.\n" +
                  $"Next Thread Will Start At {DateTime.Now.Add(dueTimeSpan).AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss")}");
                _timer.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
