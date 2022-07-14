using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Report;
using StoreService.Extensions;

namespace StoreService.Core.Service.DAL;

public class ReportService
{
    private readonly MySqlClient _mainDbClient;
    private readonly MySqlClient _reportDbClient;
    private readonly string _connectionString;

    public ReportService(IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        _connectionString = configuration.GetConnectionString("ConnectionString");
        _mainDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Report);
    }

    #region DepositPoint

     public async Task<PagerResultModel<GetDepositPointReportResult>> GetDepositPointReportAsync(GetDepositPointReportParams input)
    {
        var para = new DynamicParameters().AddPagerParams(input.PageSize, input.StartIndex);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_StartDate", input.StartTime);
        para.Add("_EndDate", input.EndTime);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        
        var list = await _reportDbClient.QueryAsync<GetDepositPointReportResult>("Store_DepositPointReport_Get", para);
        var result = new PagerResultModel<GetDepositPointReportResult>
        {
            Datas = list,
            TotalPages = para.Get<int?>("_TotalPages") ?? 0,
            RowCount = para.Get<int?>("_RowCount") ?? 0
        };
        
        return result;
    }
    
    public async Task<IEnumerable<GetDepositPointReportResult>> ExportDepositPointReportAsync(ExportDepositPointReportParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_StartDate", input.StartTime);
        para.Add("_EndDate", input.EndTime);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        
        var list = await _reportDbClient.QueryAsync<GetDepositPointReportResult>("Store_DepositPointReportExport_Get", para);
        return list;
    }
    
    public Task<IEnumerable<GetDepositPointOverviewResult>> GetDepositPointOverviewAsync(GetDepositPointReportParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        para.Add("_StartTime", input.StartTime);
        para.Add("_EndTime", input.EndTime);

        return _reportDbClient.QueryAsync<GetDepositPointOverviewResult>("Store_DepositPointOverview_Get", para);
    }

    #endregion
    
    #region Adjustment

     public async Task<PagerResultModel<GetAdjustmentPointReportResult>> GetAdjustmentPointReportAsync(GetAdjustmentPointReportParams input)
    {
        var para = new DynamicParameters().AddPagerParams(input.PageSize, input.StartIndex);
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_CategoryIds", input.StrAdjustTypes);
        para.Add("_StartDate", input.StartTime);
        para.Add("_EndDate", input.EndTime);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        
        var list = await _reportDbClient.QueryAsync<GetAdjustmentPointReportResult>("Store_AdjustmentPointReport_Get", para);
        var result = new PagerResultModel<GetAdjustmentPointReportResult>
        {
            Datas = list,
            TotalPages = para.Get<int?>("_TotalPages") ?? 0,
            RowCount = para.Get<int?>("_RowCount") ?? 0
        };
        
        return result;
    }
    
    public async Task<IEnumerable<GetAdjustmentPointReportResult>> ExportAdjustmentPointReportAsync(ExportAdjustmentPointReportParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_CategoryIds", input.StrAdjustTypes);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_StartDate", input.StartTime);
        para.Add("_EndDate", input.EndTime);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        
        var list = await _reportDbClient.QueryAsync<GetAdjustmentPointReportResult>("Store_AdjustmentPointReportExport_Get", para);
        return list;
    }
    
    public Task<IEnumerable<GetAdjustmentPointOverviewResult>> GetAdjustmentPointOverviewAsync(GetAdjustmentPointReportParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_CurrencyIds", input.StrCurrencyIds);
        para.Add("_CategoryIds", input.StrAdjustTypes);
        para.Add("_VipLevels", input.StrVipLevels);
        para.Add("_TransId", input.TransId);
        para.Add("_UserName", input.UserName);
        para.Add("_TpUserName", input.TpUserName);
        para.Add("_StartTime", input.StartTime);
        para.Add("_EndTime", input.EndTime);

        return _reportDbClient.QueryAsync<GetAdjustmentPointOverviewResult>("Store_AdjustmentPointOverview_Get", para);
    }

    #endregion
    
   
}