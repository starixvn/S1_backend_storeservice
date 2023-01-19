using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreService.Core.Service.DAL;

public class RewardPointService
{
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;
    private readonly ILogger<RewardPointService> _logger;

    public RewardPointService(IServiceProvider provider)
    {
        _logger = provider.GetRequiredService<ILogger<RewardPointService>>();
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    public async Task<(int ResultCode, string TransId)> CreateManualRewardPointInfoAsync(
        CreateManualRewardPointInfoParams input, string prefix)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_UserName", input.UserName);
        para.Add("_Point", input.Point);
        para.Add("_CategoryId", input.CategoryId);
        para.Add("_Prefix", prefix);
        para.Add("_Memo", input.Memo);
        para.Add("_Operator", input.Operator);
        para.Add("_TransId", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();
        
        _ = await _mainDbClient.ExecuteAsync("Store_ManualRewardPointInfo_Ins", trans , para );
        var resultCode = para.Get<int?>("_Result") ?? -9;
        var transId = para.Get<string>("_TransId") ?? string.Empty;

        if (resultCode == 0)
        {
            trans.Commit();
        }
        else
        {
            trans.Rollback();
        }
    

        await conn.CloseAsync();
        await conn.DisposeAsync();
        

        return (resultCode, transId);
    }

    public async Task<int> CreateManualRewardPointInfoBatchAsync(CreateManualRewardPointInfoBatchParams input,
        string prefix)
    {
        var result = -9;
        var count = input.Infos.Count();
        
        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();
        
        try
        {
            // STEP1: 批次新增手動變更點數紀錄
            var batchManualRewardPointResult = await ManualRewardPointInfoBatchAsync(input, prefix, trans);

            if (batchManualRewardPointResult.Count() < count)
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                _logger.LogError("ManualRewardPointInfoBatch Failed, data: " + JsonConvert.SerializeObject(input));

                return -1;
            }

            // STEP2: 批次變更用戶點數
            var batchChangeInfo =
                input.Infos
                    .GroupBy(x => x.CustId)
                    .Select(x =>
                        new CustRewardPointChangeInfo
                        {
                            CustId = x.Key,
                            Point = x.Sum(g => g.Point),
                            CurrencyId = x.First().CurrencyId,
                            SiteId = input.SiteId
                        });

            var batchChangeCustRewardPointResult = (await BatchChangeCustRewardPointAsync(batchChangeInfo, trans)).ToList();

            if (batchChangeCustRewardPointResult.Count != batchChangeInfo.Count())
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                _logger.LogError("BatchChangeCustRewardPoint Failed, data: " + JsonConvert.SerializeObject(batchChangeInfo));

                return -2;
            }

            // STEP3: 批次新增用戶點數歷程
            var batchLogInfo = new List<BatchRewardPointLogInfo>();
            batchChangeCustRewardPointResult.ForEach(change =>
            {
                var manual = batchManualRewardPointResult
                            .Where(x => x.CustId == change.CustId)
                            .OrderBy(x => x.TransId);

                var before = change.BeforePoint;
                batchLogInfo.AddRange(manual.Select(item => new BatchRewardPointLogInfo
                {
                    CustId = change.CustId,
                    CurrencyId = item.CurrencyId,
                    SiteId = input.SiteId,
                    TransId = item.TransId,
                    ChangePoint = item.Point,
                    BeforePoint = before,
                    AfterPoint = before += item.Point,
                    Memo = item.Memo
                }));
            });

            var logResult = await BatchRewardPointLogAsync(PointTypeEnum.Manual, input.Operator, batchLogInfo, trans);

            if (logResult != batchLogInfo.Count)
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                _logger.LogError("BatchRewardPointLog Failed, data: " + JsonConvert.SerializeObject(batchLogInfo));

                return -3;
            }

            result = logResult;
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            await conn.CloseAsync();
            await conn.DisposeAsync();
            throw;
        }
        
        await conn.CloseAsync();
        await conn.DisposeAsync();

        return result;
    }

    public Task<IEnumerable<DepositRewardPointQueue>> GetDepositRewardPointQueueAsync()
    {
        return _reportDbClient.QueryAsync<DepositRewardPointQueue>("Store_DepositRewardPointQueue_Get");
    }

    public async Task<int> DepositBatchRewardPointAsync(DepositBatchRewardPointParams input)
    {
        int result;
        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();
        
        try
        {
            // STEP1: 批次更新存款佇列

            var updateQueueResult = await SetDepositRewardPointQueueAsync(input.DepositQueueInfos, trans);
            if (updateQueueResult < input.DepositQueueInfos.Count())
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                return -1;
            }

            // STEP2: 批次變更用戶點數
            var batchChangeInfo =
                input.DepositRewardPointInfos
                    .GroupBy(x => x.CustId)
                    .Select(x =>
                        new CustRewardPointChangeInfo
                        {
                            CustId = x.Key,
                            Point = x.Sum(g => g.Point),
                            CurrencyId = input.CurrencyId,
                            SiteId = x.First().SiteId
                        });

            var batchChangeCustRewardPointResult = await BatchChangeCustRewardPointAsync(batchChangeInfo, trans);
            if (batchChangeCustRewardPointResult.Count() != input.DepositRewardPointInfos.Count())
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                return -2;
            }

            // STEP3: 批次新增用戶點數歷程
            var batchLogInfo =
                input.DepositRewardPointInfos
                    .Join(batchChangeCustRewardPointResult,
                        x => x.CustId,
                        y => y.CustId,
                        (x, y) => new BatchRewardPointLogInfo
                        {
                            CustId = x.CustId,
                            CurrencyId = input.CurrencyId,
                            SiteId = x.SiteId,
                            TransId = x.TransId,
                            ChangePoint = x.Point,
                            BeforePoint = y.BeforePoint,
                            AfterPoint = y.AfterPoint,
                            Memo = x.Memo
                        })
                    .Distinct(new BatchRewardPointLogInfoComparer());

            var logResult = await BatchRewardPointLogAsync(PointTypeEnum.Deposit, "System", batchLogInfo, trans);

            if (logResult != batchLogInfo.Count())
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                return -3;
            }

            result = logResult;
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            await conn.CloseAsync();
            await conn.DisposeAsync();
            throw;
        }
        
        await conn.CloseAsync();
        await conn.DisposeAsync();

        return result;
    }

    public async Task<bool> CustRewardPointUseAsync(CustRewardPointUseParams input, string transId, IDbTransaction trans)
    {
        var isUsed = Math.Sign(input.UsePoint);
        var para = new DynamicParameters();
        para.Add("_CustId", input.CustId);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_UsePoint", input.UsePoint);
        para.Add("_BeforeBalance", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_AfterBalance", dbType: DbType.String, direction: ParameterDirection.Output);

        var result = await _mainDbClient.ExecuteAsync("Store_CustRewardPointUse_Set", trans , para );

        if (result > 0)
        {
            var beforeBalance = para.Get<decimal>("_BeforeBalance");
            var afterBalance = para.Get<decimal>("_AfterBalance");

            var logPara = new DynamicParameters();
            logPara.Add("_PointType", (int)PointTypeEnum.Redemption);
            logPara.Add("_CustId", input.CustId);
            logPara.Add("_CurrencyId", input.CurrencyId);
            logPara.Add("_SiteId", input.SiteId);
            logPara.Add("_TransId", transId);
            logPara.Add("_ChangePoint",  input.UsePoint * -1);
            logPara.Add("_BeforePoint", beforeBalance);
            logPara.Add("_AfterPoint", afterBalance);
            logPara.Add("_Memo", isUsed >= 0 ? "Apply Redemption" : "Reject Redemption");
            logPara.Add("_Operator", "System");

            var logResult = await _mainDbClient.ExecuteAsync("Store_RewardPointLog_Ins", trans , logPara);
            if (logResult > 0)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<int> SetPointExpiryAsync(IEnumerable<CustomerInfo> custList)
    {
        var result = -9;
        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();
        
        try
        {
            // STEP1: 批次歸零點數
            var custIds = custList.Select(x => x.CustId);
            var (changeResult, rowCount) = await BatchExpiryCustRewardPointAsync(custIds, trans);

            if (rowCount != custList.Count())
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                _logger.LogError("BatchExpiryCustRewardPoint Failed.");

                return -1;
            }

            // STEP2: 批次新增用戶點數歷程
            var batchLogInfo = custList
                .Join(changeResult,
                    x => x.CustId,
                    y => y.CustId,
                    (x, y) => new BatchRewardPointLogInfo
                    {
                        CustId = x.CustId,
                        CurrencyId = x.CurrencyId,
                        SiteId = x.SiteId,
                        TransId = string.Empty,
                        ChangePoint = y.BeforePoint * -1,
                        BeforePoint = y.BeforePoint,
                        AfterPoint = y.AfterPoint,
                        Memo = "Point Expired"
                    });

            var logResult = await BatchRewardPointLogAsync(PointTypeEnum.System, "System", batchLogInfo, trans);

            if (logResult != batchLogInfo.Count())
            {
                trans.Rollback();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                _logger.LogError("BatchRewardPointLog Failed, data: " + JsonConvert.SerializeObject(batchLogInfo));

                return -2;
            }

            result = logResult;
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            await conn.CloseAsync();
            await conn.DisposeAsync();
            throw;
        }

        await conn.CloseAsync();
        await conn.DisposeAsync();

        return result;
    }

    public async Task<bool> CustRewardPointChange(CustRewardPointChangeParams input, IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_CustId", input.CustId);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_ChangPoint", input.ChangePoint);
        para.Add("_BeforeBalance", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_AfterBalance", dbType: DbType.String, direction: ParameterDirection.Output);
        para.Add("_Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _mainDbClient.ExecuteAsync("Store_CustRewardPointChange_Set", trans, para);

        var result = para.Get<int?>("_Result") ?? -9;
        var beforeBalance = para.Get<decimal>("_BeforeBalance");
        var afterBalance = para.Get<decimal>("_AfterBalance");

        if (result == 0)
        {
            var logPara = new DynamicParameters();
            logPara.Add("_PointType", (int)PointTypeEnum.Turnover);
            logPara.Add("_CustId", input.CustId);
            logPara.Add("_CurrencyId", input.CurrencyId);
            logPara.Add("_SiteId", input.SiteId);
            logPara.Add("_TransId", input.TransId);
            logPara.Add("_ChangePoint", input.ChangePoint);
            logPara.Add("_BeforePoint", beforeBalance);
            logPara.Add("_AfterPoint", afterBalance);
            logPara.Add("_Memo", input.Memo);
            logPara.Add("_Operator", "System");

            var logResult = await _mainDbClient.ExecuteAsync("Store_RewardPointLog_Ins", trans , logPara);

            if (logResult > 0)
            {
                return true;
            }
        }

        return false;
    }

    #region Private Methods

    private Task<IEnumerable<BatchManualRewardPointResult>> ManualRewardPointInfoBatchAsync(
        CreateManualRewardPointInfoBatchParams input, string prefix, IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_Infos", JsonConvert.SerializeObject(input.Infos));
        para.Add("_Prefix", prefix);
        para.Add("_Operator", input.Operator);

        var result = _mainDbClient.QueryAsync<BatchManualRewardPointResult>("Store_ManualRewardPointInfoBatch_Ins", trans ,para);

        return result;
    }

    private Task<IEnumerable<BatchChangeCustRewardPointResult>> BatchChangeCustRewardPointAsync(
        IEnumerable<CustRewardPointChangeInfo> batchChangeInfo, IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_Infos", JsonConvert.SerializeObject(batchChangeInfo));

        return _mainDbClient.QueryAsync<BatchChangeCustRewardPointResult>("Store_CustRewardPointChangeBatch_Set", trans , para);
    }

    private Task<int> BatchRewardPointLogAsync(PointTypeEnum type, string @operator,
        IEnumerable<BatchRewardPointLogInfo> batchLogInfo, IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_PointType", (int) type);
        para.Add("_Infos", JsonConvert.SerializeObject(batchLogInfo));
        para.Add("_Operator", @operator);

        return _mainDbClient.ExecuteAsync("Store_RewardPointLogBatch_Ins", trans , para);
    }

    private Task<int> SetDepositRewardPointQueueAsync(
        IEnumerable<DepositRewardPointQueueInfo> infos, IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_Infos", JsonConvert.SerializeObject(infos));

        return _mainDbClient.ExecuteAsync("Store_DepositRewardPointQueue_Set", trans , para);
    }

    private async Task<(IEnumerable<BatchChangeCustRewardPointResult> ChangeResult, int RowCount)>
        BatchExpiryCustRewardPointAsync(IEnumerable<int> custIds,IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_CustIds", JsonConvert.SerializeObject(custIds));
        para.Add("_EffectedRowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

        var result = await _mainDbClient.QueryAsync<BatchChangeCustRewardPointResult>("Store_CustRewardPointExpiryBatch_Set", trans ,para);
        var rowCount = para.Get<int?>("_EffectedRowCount") ?? 0;
        return (result, rowCount);
    }

    #endregion
}