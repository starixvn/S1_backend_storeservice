using Newtonsoft.Json;
using StoreHangfireService.Job;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;
using StoreService.Core.Service.Model.PointSetting;
using StoreService.Core.Service.Model;
using System.Diagnostics;
using System.Threading.Tasks;
using StoreService.Core.Service.Model.TurnoverWorker;
using System;
using System.Drawing.Printing;

namespace StoreHangfireService.Service.Reward;

public class TurnoverRewardService
{
    private readonly TurnoverWorkerBLL _turnoverWorkerBll;
    private readonly ILogger<BaseJob> _logger;

    public TurnoverRewardService(IServiceProvider provider)
    {
        _turnoverWorkerBll = provider.GetRequiredService<TurnoverWorkerBLL>();
        _logger = provider.GetRequiredService<ILogger<BaseJob>>();
    }

    public async Task RunJobAsync()
    {
        _logger.LogInformation("TurnoverWorker：Verify Task Start.");

        var doTaskList = await VerifyTask();

        _logger.LogInformation($"TurnoverWorker : Verify Task End. Count = {doTaskList.Count()}");

        foreach (var task in doTaskList)
        {
            _logger.LogWarning($"TurnoverWorker : Do Task Start. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.");
            var sw = Stopwatch.StartNew();

            var pointSetting = await GetSetting(task.SiteId, task.CurrencyId, task.TimeZone);

            var custList = await GetCustList(task.SiteId, task.CurrencyId, task.TimeZone);
            if (custList.Any())
                if (!await CaculatePoint(task, custList, pointSetting))
                {
                    _logger.LogError($"TurnoverWorker : CaculatePoint Fail. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.");
                    throw new Exception();
                }

            if (!await UpdateTaskSettledTIme(task.SiteId, task.CurrencyId))
            {
                _logger.LogError($"TurnoverWorker : Update Task SettleTime Fail. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.");
                throw new Exception();
            }

            _logger.LogInformation($"TurnoverWorker : Do Task End. SiteId:{task.SiteId} CurrencyId:{task.CurrencyId}.\n"
            + $"Finish Time (m:s.ms): {Math.Round(sw.Elapsed.TotalMinutes)}:{Math.Round(sw.Elapsed.TotalMinutes)}.{Math.Round(sw.Elapsed.TotalMilliseconds)}.");
        }
        await Task.CompletedTask;
    }

    private async Task<IEnumerable<TurnoverPeriodForWorkerUse>> VerifyTask()
    => await _turnoverWorkerBll.VerifyTaskAsync();

    private async Task<IEnumerable<TurnoverSettingForWorkerUse>> GetSetting(int siteId, int currencyId, string timeZone)
        => await _turnoverWorkerBll.GetSettingAsync(siteId, currencyId, timeZone);
    private async Task<IEnumerable<TurnoverCustListModel>> GetCustList(int siteId, int currencyId, string timeZone)
        => await _turnoverWorkerBll.GetCustListAsync(siteId, currencyId, timeZone);

    private async Task<bool> CaculatePoint(TurnoverPeriodForWorkerUse task,
        IEnumerable<TurnoverCustListModel> custList,
        IEnumerable<TurnoverSettingForWorkerUse> pointSetting)
        => await _turnoverWorkerBll.CaculatePointAsync(task, custList, pointSetting);

    private async Task<bool> UpdateTaskSettledTIme(int siteId, int currencyId)
        => await _turnoverWorkerBll.UpdateTaskSettledTImeASync(siteId, currencyId);
}
