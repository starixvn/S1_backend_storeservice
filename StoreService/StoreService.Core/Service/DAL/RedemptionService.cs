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
using StoreService.Extensions;

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
                var pointUseResult = await _rewardPointService.CustRewardPointUseAsync(custRewardPointUseRequest,transId,conn,trans);
                    
                if (pointUseResult)
                {
                    trans.Commit();
                    return true;
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
    
    public async Task<int> CheckCustRedemptionLimitAsync(int siteId, int currencyId , int custId ,int point)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", siteId);
        para.Add("_CurrencyId", currencyId);
        para.Add("_CustId", custId);
        para.Add("_Point", point);
        para.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await _reportDbClient.ExecuteAsync("Store_CustRedemptionLimitCheck_Get", para);
        var result = para.Get<int?>("Result") ?? 0;
        
        return result;
    }
    
    public async Task<PagerResultModel<GetRedemptionListResult>> GetRedemptionListAsync(GetRedemptionListParams input)
    {
        var para = new DynamicParameters().AddPagerParams(input.PageSize, input.StartIndex);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_Status", input.StrStatuses);
        para.Add("_StartDate", input.StartTime);
        para.Add("_EndDate", input.EndTime);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        
        var list = await _reportDbClient.QueryAsync<GetRedemptionListResult>("Store_RedemptionList_Get", para);
        var result = new PagerResultModel<GetRedemptionListResult>
        {
            Datas = list,
            TotalPages = para.Get<int?>("_TotalPages") ?? 0,
            RowCount = para.Get<int?>("_RowCount") ?? 0
        };
        
        return result;
    }
    
    public Task<IEnumerable<GetRedemptionOverviewResult>> GetRedemptionOverviewAsync(GetRedemptionListParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_Status", input.StrStatuses);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        para.Add("_StartTime", input.StartTime);
        para.Add("_EndTime", input.EndTime);

        return _reportDbClient.QueryAsync<GetRedemptionOverviewResult>("Store_RedemptionOverview_Get", para);
    }
    
    public async Task<IEnumerable<GetRedemptionListResult>> ExportRedemptionListAsync(ExportRedemptionListParams input )
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_Status", input.StrStatuses);
        para.Add("_StartDate", input.StartTime);
        para.Add("_EndDate", input.EndTime);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        
        var results = await _reportDbClient.QueryAsync<GetRedemptionListResult>("Store_RedemptionListExport_Get", para);

        return results;
    }
    
    public async Task<GetRedemptionDetail> GetRedemptionDetailAsync(GetRedemptionDetailRequest input )
    {
        var para = new DynamicParameters();
        para.Add("_RedemptionId", input.RedemptionId);
        var result = await _reportDbClient.QueryFirstAsync<GetRedemptionDetail>("Store_RedemptionInfoById_Get", para);
        
        return result;
    }
    
    public async Task<IEnumerable<GetRedemptionLog>> GetRedemptionLogAsync(GetRedemptionLogRequest input )
    {
        var para = new DynamicParameters();
        para.Add("_TransId", input.TransId);
        var result = await _reportDbClient.QueryAsync<GetRedemptionLog>("Store_RedemptionLog_Get", para);
        
        return result;
    }
    
    public async Task<GetRedemptionByTransIdResult> GetRedemptionByTransIdAsync(string transId )
    {
        var para = new DynamicParameters();
        para.Add("_TransId", transId);
        var result = await _reportDbClient.QueryAsync<GetRedemptionByTransIdResult>("Store_RedemptionByTransId_Get", para);
        
        return result.FirstOrDefault();
    }
    
    public async Task<bool> SetRedemptionInfoAsync(SetRedemptionInfoRequest input ,GetRedemptionByTransIdResult info)
    {
        var success = false;
        await using var conn = new MySqlConnection(_connectionString);
        conn.Open();

        await using (var trans = conn.BeginTransaction())
        {
            try
            {
                var action = input.Action == (int) RedemptionEnum.Draft ? info.Status : input.Action;
                var receiptUrls = input.ReceiptUrls.Any() ? JsonConvert.SerializeObject(input.ReceiptUrls) : "";
                
                var para = new DynamicParameters();
                para.Add("_TransId", input.TransId);
                para.Add("_Status", action);
                para.Add("_ActualCost", input.ActualCost);
                para.Add("_ReceiptUrls", receiptUrls);
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
