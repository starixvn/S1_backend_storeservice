using System.Data;

using Dapper;

using Microsoft.Extensions.DependencyInjection;

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

    public RedemptionService(IServiceProvider provider)
    {
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
        _rewardPointService = provider.GetRequiredService<RewardPointService>();
    }

    public async Task<bool> CreateRedemptionAsync(CreateRedemptionParams input)
    {
        var para = new DynamicParameters();
        para.Add("_GiftId", input.GiftId);
        para.Add("_CustId", input.CustId);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyId", input.CurrencyId);
        para.Add("_Point", input.Point);
        para.Add("_Prefix", "RS");
        para.Add("_TransId", dbType: DbType.String, direction: ParameterDirection.Output);

        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();

        _ = _mainDbClient.ExecuteAsync("Store_RedemptionInfo_Ins", trans, para);
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
            var pointUseResult = await _rewardPointService.CustRewardPointUseAsync(custRewardPointUseRequest, transId, trans);

            if (pointUseResult)
            {
                trans.Commit();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                return true;
            }

            trans.Rollback();
        }
        else
        {
            trans.Rollback();
        }

        await conn.CloseAsync();
        await conn.CloseAsync();

        return false;
    }

    public async Task<int> CheckCustRedemptionLimitAsync(int siteId, int currencyId, int custId, int point)
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

    public async Task<IEnumerable<GetRedemptionListResult>> ExportRedemptionListAsync(ExportRedemptionListParams input)
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

    public async Task<GetRedemptionDetail> GetRedemptionDetailAsync(GetRedemptionDetailRequest input)
    {
        var para = new DynamicParameters();
        para.Add("_RedemptionId", input.RedemptionId);
        var result = await _reportDbClient.QueryFirstAsync<GetRedemptionDetail>("Store_RedemptionInfoById_Get", para);

        return result;
    }

    public async Task<IEnumerable<GetRedemptionLog>> GetRedemptionLogAsync(GetRedemptionLogRequest input)
    {
        var para = new DynamicParameters();
        para.Add("_TransId", input.TransId);
        var result = await _reportDbClient.QueryAsync<GetRedemptionLog>("Store_RedemptionLog_Get", para);

        return result;
    }

    public async Task<GetRedemptionByTransIdResult> GetRedemptionByTransIdAsync(string transId)
    {
        var para = new DynamicParameters();
        para.Add("_TransId", transId);
        var result = await _reportDbClient.QueryAsync<GetRedemptionByTransIdResult>("Store_RedemptionByTransId_Get", para);

        return result.FirstOrDefault();
    }

    public async Task<bool> SetRedemptionInfoAsync(SetRedemptionInfoRequest input, GetRedemptionByTransIdResult info)
    {
        var success = false;
        await using var conn = _mainDbClient.GetMySqlConnection();
        conn.Open();
        await using var trans = await conn.BeginTransactionAsync();

        try
        {
            var action = input.Action == (int)RedemptionEnum.Draft ? info.Status : input.Action;
            var receiptUrls = input.ReceiptUrls.Any() ? JsonConvert.SerializeObject(input.ReceiptUrls) : "";

            var para = new DynamicParameters();
            para.Add("_TransId", input.TransId);
            para.Add("_Status", action);
            para.Add("_ActualCost", input.ActualCost);
            para.Add("_ReceiptUrls", receiptUrls);
            para.Add("_Operator", input.Operator);
            var setResult = _mainDbClient.ExecuteAsync("Store_RedemptionInfo_Set", trans, para);

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

                var logResult = SetRedemptionLog(logRequest, trans);

                if (logResult > 0)
                {
                    success = true;
                }

                if (action == (int)RedemptionEnum.Rejected)
                {
                    var custRewardPointUseRequest = new CustRewardPointUseParams
                    {
                        CustId = info.CustId,
                        CurrencyId = info.CurrencyId,
                        SiteId = input.SiteId,
                        UsePoint = -info.Point
                    };
                    var pointUseResult = await _rewardPointService.CustRewardPointUseAsync(custRewardPointUseRequest, info.TransId, trans);

                    if (!pointUseResult)
                    {
                        success = false;
                    }
                }

                if (!success)
                    trans.Rollback();
                else
                {
                    trans.Commit();
                }
            }
        }
        catch (Exception e)
        {
            trans.Rollback();
            await conn.CloseAsync();
            throw;
        }

        await conn.CloseAsync();
        await conn.DisposeAsync();

        return success;
    }


    public async Task<PagerResultModel<GetMyPurchesResult>> GetMyPurchesAsync(GetMyPurchesParams input)
    {
        var para = new DynamicParameters().AddPagerParams(input.PageSize, input.StartIndex);
        para.Add("_CustId", input.CustId);
        para.Add("_Lang", input.Lang);

        var list = await _reportDbClient.QueryAsync<GetMyPurchesResult>("Store_RedemptionByCustId_Get", para);
        var result = new PagerResultModel<GetMyPurchesResult>
        {
            Datas = list,
            TotalPages = para.Get<int?>("_TotalPages") ?? 0,
            RowCount = para.Get<int?>("_RowCount") ?? 0
        };

        return result;
    }

    #region private

    private int SetRedemptionLog(SetRedemptionLogParams input, IDbTransaction trans)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_TransId", input.TransId);
        para.Add("_Action", input.Action);
        para.Add("_Comment", input.Comment);
        para.Add("_Operator", input.Operator);
        var setResult = _mainDbClient.Execute("Store_RedemptionLog_Ins", trans, para);

        return setResult;
    }

    #endregion

}
