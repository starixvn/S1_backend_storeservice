using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Redemption;
using StoreService.Core.Service.Model.RewardPoint;

namespace StoreService.Core.Service.DAL;

public class RedemptionService
{
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;
    private readonly RewardPointService _rewardPointService;
    private readonly string _connectionString;

    public RedemptionService(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        _connectionString = configuration.GetConnectionString("ConnectionString");
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
        _rewardPointService = provider.GetRequiredService<RewardPointService>();
    }
    
    public async Task<bool> CreateRedemptionAsync(CreateRedemptionParams input )
    {
        var para = new DynamicParameters();
        para.Add("_GiftId", input.GiftId);
        para.Add("_CustId", input.CustId);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Point", input.Point);
        para.Add("_Prefix", "RS");
        para.Add("_TransId", dbType: DbType.String, direction: ParameterDirection.Output);

        await using var conn = new MySqlConnection(_connectionString);
        conn.Open();

        await using (var trans = conn.BeginTransaction())
        {
            _ = conn.ExecuteAsync("Store_RedemptionInfo_Ins", para, trans, commandType: CommandType.StoredProcedure);
            var transId = para.Get<string>("_TransId") ?? string.Empty;

            if (!string.IsNullOrEmpty(transId))
            {
                var custRewardPointUseRequest = new CustRewardPointUseParams
                {
                    CustId = input.CustId,
                    CurrencyId = input.CurrencyId,
                    SiteId = input.SiteId,
                    UsePoint = input.Point
                };
                var pointUseResult = await _rewardPointService.CustRewardPointUse(custRewardPointUseRequest,transId,conn,trans);
                    
                if (pointUseResult)
                {
                    trans.Commit();
                    return pointUseResult;
                }

                trans.Rollback();
            }
            else
            {
                trans.Rollback();
            }
        }

        await conn.CloseAsync();

        return false;
    }
    
    public async Task<PagerResultModel<GetRedemptionListResult>> GetRedemptionListAsync(GetRedemptionListRequest input )
    {
        var selectSite = JsonConvert.SerializeObject(input.SiteIds);
        var selectCurrencyId = JsonConvert.SerializeObject(input.CurrencyIds);
        var selectVipLevels = JsonConvert.SerializeObject(input.VipLevels);
        var selectStatuses = JsonConvert.SerializeObject(input.Statuses);
        
        var para = new DynamicParameters();
        para.Add("_SelectSite", selectSite);
        para.Add("_SelectCurrencyId", selectCurrencyId);
        para.Add("_VipLevels", selectVipLevels);
        para.Add("_Status", selectStatuses);
        para.Add("_StartDate", input.StartTime?.DateTime);
        para.Add("_EndDate", input.EndTime?.DateTime);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_SabaId", input.SabaId);
        para.Add("_StartIndex", (input.PageNumber - 1) * input.PageSize);
        para.Add("_PageSize", input.PageSize);
        para.Add("TotalPages", dbType: DbType.Int32, direction: ParameterDirection.Output);
        para.Add("RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
        
        var list = await _reportDbClient.QueryAsync<GetRedemptionListResult>("Store_RedemptionList_Get", para);
        var result = new PagerResultModel<GetRedemptionListResult>
        {
            Datas = list,
            TotalPages = para.Get<int?>("TotalPages") ?? 0,
            RowCount = para.Get<int?>("RowCount") ?? 0
        };
        
        return result;
    }
    
    public async Task<GetRedemptionDetail> GetRedemptionDetailAsync(GetRedemptionDetailRequest input )
    {
        var para = new DynamicParameters();
        para.Add("_RedemptionId", input.RedemptionId);
        var result = await _reportDbClient.QueryFirstAsync<GetRedemptionDetail>("Store_RedemptionInfoById_Get", para);
        
        return result;
    }
    
    public async Task<GetRedemptionByTransIdResult> GetRedemptionByTransIdAsync(string transId )
    {
        var para = new DynamicParameters();
        para.Add("_TransId", transId);
        var result = await _reportDbClient.QueryAsync<GetRedemptionByTransIdResult>("Store_RedemptionByTransId_Get", para);
        
        return result.FirstOrDefault();
    }
    
    public async Task<bool> SetRedemptionInfoAsync(SetRedemptionInfoRequest input )
    {
        var success = false;
        await using var conn = new MySqlConnection(_connectionString);
        conn.Open();

        await using (var trans = conn.BeginTransaction())
        {
            try
            {
                var para = new DynamicParameters();
                para.Add("_TransId", input.TransId);
                para.Add("_Status", input.Action);
                para.Add("_ActualCost", input.ActualCost);
                para.Add("_ReceiptUrls", input.ReceiptUrls);
                para.Add("_Operator", input.Operator);
                var setResult = conn.ExecuteAsync("Store_RedemptionInfo_Set", para, trans, commandType: CommandType.StoredProcedure);
            
                if (setResult.Result > 0)
                {
                    var logRequest = new SetRedemptionLogParams
                    {
                        SiteId = input.SiteId,
                        TransId = input.TransId,
                        Action = input.Action,
                        Comment = input.Comment,
                        Operator = input.Operator
                    };
                
                    var logResult = SetRedemptionLog(logRequest,conn,trans);
                    
                    if (logResult > 0)
                    {
                        trans.Commit();
                        success = true;
                    
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
                else
                {
                    trans.Rollback();
                }
            }
            catch (Exception e)
            {
                trans.Rollback();
                await conn.CloseAsync();
                throw;
            }
            
        }
        await conn.CloseAsync();
        
        return success ;
    }


    #region private

    private int SetRedemptionLog(SetRedemptionLogParams input ,MySqlConnection conn, MySqlTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_TransId", input.TransId);
        para.Add("_Action", input.Action);
        para.Add("_Comment", input.Comment);
        para.Add("_Operator", input.Operator);
        var setResult = conn.Execute("Store_RedemptionLog_Ins", para, trans, commandType: CommandType.StoredProcedure);

        return setResult;
    }

    #endregion
    
}
