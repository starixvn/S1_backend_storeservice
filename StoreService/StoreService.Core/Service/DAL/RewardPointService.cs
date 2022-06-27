using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreService.Core.Service.DAL;

public class RewardPointService
{
    private readonly IConfiguration _configuration;
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;
    private readonly string _connectionString;
    
    public RewardPointService(IServiceProvider provider)
    {
        _configuration = provider.GetRequiredService<IConfiguration>();
        _connectionString = _configuration.GetConnectionString("ConnectionString");
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }
    
    public async Task<(int ResultCode, string TransId)> CreateManualRewardPointInfoAsync(CreateManualRewardPointInfoParams input, string prefix)
    {
        var resultCode = -9;
        var transId = string.Empty;
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

        await using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();

            await using (var trans = conn.BeginTransaction())
            {
                _ = await conn.ExecuteAsync("Store_ManualRewardPointInfo_Ins", para, transaction: trans,
                    commandType: CommandType.StoredProcedure);
                resultCode = para.Get<int?>("_Result") ?? -9;
                transId = para.Get<string>("_TransId") ?? string.Empty;
                
                if (resultCode == 0)
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                }
            }

            await conn.CloseAsync();
        }

        return (resultCode, transId);
    }
    
    public async Task<int> CreateManualRewardPointInfoBatchAsync(CreateManualRewardPointInfoBatchParams input, string prefix)
    {
        var result = 0;
        var count = input.Infos.Count();
        await using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();
            await using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // STEP1: 批次新增手動變更點數紀錄
                    var para = new DynamicParameters();
                    para.Add("_SiteId", input.SiteId);
                    para.Add("_Infos", JsonConvert.SerializeObject(input.Infos));
                    para.Add("_Prefix", prefix);
                    para.Add("_Operator", input.Operator);
                    
                    var batchManualRewardPointResult =
                        await conn.QueryAsync<BatchManualRewardPointResult>(
                            "Store_ManualRewardPointInfoBatch_Ins",
                            para, transaction:
                            trans,
                            commandType: CommandType.StoredProcedure);

                    if (batchManualRewardPointResult.Count() < count)
                    {
                        trans.Rollback();
                        await conn.CloseAsync();
                        return -1;
                    }

                    // STEP2: 批次變更用戶點數
                    var batchChangeInfo =
                        input.Infos.Select(x =>
                            new
                            {
                                x.CustId,
                                x.Point,
                                x.CurrencyId,
                                input.SiteId
                            });

                    para = new DynamicParameters();
                    para.Add("_Infos", JsonConvert.SerializeObject(batchChangeInfo));
                    
                    var batchChangeCustRewardPointResult = 
                        await conn.QueryAsync<BatchChangeCustRewardPointResult>(
                            "Store_CustRewardPointChangeBatch_Set",
                            para, transaction:
                            trans,
                            commandType: CommandType.StoredProcedure);
                    
                    if (batchChangeCustRewardPointResult.Count() != count)
                    {
                        trans.Rollback();
                        await conn.CloseAsync();
                        return -2;
                    }
                    
                    // STEP3: 批次新增用戶點數歷程
                    var batchLogInfo =
                        batchManualRewardPointResult
                            .Join(batchChangeCustRewardPointResult,
                                x => x.CustId,
                                y => y.CustId,
                                (x, y) => new BatchRewardPointLogInfo
                                {
                                    CustId = x.CustId,
                                    CurrencyId = x.CurrencyId,
                                    SiteId = input.SiteId,
                                    TransId = x.TransId,
                                    ChangePoint = x.Point,
                                    BeforePoint = y.BeforePoint,
                                    AfterPoint = y.AfterPoint,
                                    Memo = x.Memo
                                })
                            .Distinct(new BatchRewardPointLogInfoComparer());
                    
                    para = new DynamicParameters();
                    para.Add("_PointType", (int) PointTypeEnum.Manual);
                    para.Add("_Infos", JsonConvert.SerializeObject(batchLogInfo));
                    para.Add("_Operator", input.Operator);
                    
                    var logResult = 
                        await conn.ExecuteAsync(
                            "Store_RewardPointLogBatch_Ins",
                            para, transaction:
                            trans,
                            commandType: CommandType.StoredProcedure);
                    
                    if (logResult != count)
                    {
                        trans.Rollback();
                        await conn.CloseAsync();
                        return -3;
                    }

                    result = logResult;
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    await conn.CloseAsync();
                    throw;
                }
            }

            await conn.CloseAsync();
        }

        return result;
    }
    
    
    public Task<IEnumerable<DepositRewardPointQueue>> GetDepositRewardPointQueueAsync()
    {
        return _reportDbClient.QueryAsync<DepositRewardPointQueue>("Store_DepositRewardPointQueue_Get");
    }
    
    public async Task<int> DepositBatchRewardPointAsync(DepositBatchRewardPointParams input)
    {
        var result = 0;
        await using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();
            await using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // STEP1: 批次更新存款佇列
                    var para = new DynamicParameters();
                    para.Add("_Infos", JsonConvert.SerializeObject(input.DepositQueueInfos));
                    
                    var updateQueueResult =
                        await conn.ExecuteAsync(
                            "Store_DepositRewardPointQueue_Set",
                            para, transaction:
                            trans,
                            commandType: CommandType.StoredProcedure);

                    if (updateQueueResult < input.DepositQueueInfos.Count())
                    {
                        trans.Rollback();
                        await conn.CloseAsync();
                        return -1;
                    }

                    // STEP2: 批次變更用戶點數
                    var batchChangeInfo =
                        input.DepositRewardPointInfos.Select(x =>
                            new
                            {
                                x.CustId,
                                x.Point,
                                input.CurrencyId,
                                x.SiteId
                            });

                    para = new DynamicParameters();
                    para.Add("_Infos", JsonConvert.SerializeObject(batchChangeInfo));
                    
                    var batchChangeCustRewardPointResult = 
                        await conn.QueryAsync<BatchChangeCustRewardPointResult>(
                            "Store_CustRewardPointChangeBatch_Set",
                            para, transaction:
                            trans,
                            commandType: CommandType.StoredProcedure);
                    
                    if (batchChangeCustRewardPointResult.Count() != input.DepositRewardPointInfos.Count())
                    {
                        trans.Rollback();
                        await conn.CloseAsync();
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
                                    Memo = "deposit reward point"
                                })
                            .Distinct(new BatchRewardPointLogInfoComparer());
                    
                    para = new DynamicParameters();
                    para.Add("_PointType", (int) PointTypeEnum.Manual);
                    para.Add("_Infos", JsonConvert.SerializeObject(batchLogInfo));
                    para.Add("_Operator", "Sysem");
                    
                    var logResult = 
                        await conn.ExecuteAsync(
                            "Store_RewardPointLogBatch_Ins",
                            para, transaction:
                            trans,
                            commandType: CommandType.StoredProcedure);
                    
                    if (logResult != batchLogInfo.Count())
                    {
                        trans.Rollback();
                        await conn.CloseAsync();
                        return -3;
                    }

                    result = logResult;
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    await conn.CloseAsync();
                    throw;
                }
            }

            await conn.CloseAsync();
        }

        return result;
    }
}