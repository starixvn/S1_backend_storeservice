using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Member;
using StoreService.Extensions;

namespace StoreService.Core.Service.DAL;

public class MemberService
{
    
    private readonly IConfiguration _configuration;
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;
    private readonly string _connectionString;

    public MemberService(IServiceProvider provider)
    {
        _configuration = provider.GetRequiredService<IConfiguration>();
        _connectionString = _configuration.GetConnectionString("ConnectionString");
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }
    
    public async Task<PagerResultModel<MemberRewardPointInfo>> GetMemberRewardPointListAsync(GetListParams input)
    {
        var para = new DynamicParameters()
            .AddPagerParams(input.PageSize, input.StartIndex);

        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_UserName", string.IsNullOrEmpty(input.UserName) ? null : input.UserName, DbType.String);
        para.Add("_TpUserName", string.IsNullOrEmpty(input.TpUserName) ? null : input.TpUserName, DbType.String);
        para.Add("_StartTime", input.StartTime);
        para.Add("_EndTime", input.EndTime);

        var list = await _reportDbClient.QueryAsync<MemberRewardPointInfo>("Store_CustRewardPointList_Get", para);
        var result = new PagerResultModel<MemberRewardPointInfo>
        {
            Datas = list,
            TotalPages = para.Get<int?>("_TotalPages") ?? 0,
            RowCount = para.Get<int?>("_RowCount") ?? 0
        };

        return result;
    }
    
    public async Task<PagerResultModel<MemberRewardPointLog>> GetMemberRewardPointLogAsync(GetMemberRewardPointLogParams input)
    {
        var para = new DynamicParameters()
            .AddPagerParams(input.PageSize, input.StartIndex);

        para.Add("_CustId", input.CustId);
        para.Add("_StartTime", input.StartTime);
        para.Add("_EndTime", input.EndTime);

        var list = await _reportDbClient.QueryAsync<MemberRewardPointLog>("Store_RewardPointLog_Get", para);
        var result = new PagerResultModel<MemberRewardPointLog>
        {
            Datas = list,
            TotalPages = para.Get<int?>("_TotalPages") ?? 0,
            RowCount = para.Get<int?>("_RowCount") ?? 0
        };

        return result;
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
                                x.Point
                            });

                    para = new DynamicParameters();
                    para.Add("_SiteId", input.SiteId);
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
                                (x, y) => new
                                {
                                    x.CustId,
                                    x.CurrencyId,
                                    input.SiteId,
                                    x.TransId,
                                    ChangePoint = x.Point,
                                    y.BeforePoint,
                                    y.AfterPoint,
                                    x.Memo
                                });
                    
                    para = new DynamicParameters();
                    para.Add("_PointType", (int) PointTypeEnum.Manual);
                    para.Add("_SiteId", input.SiteId);
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
}