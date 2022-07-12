using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model;
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
    
    public Task<IEnumerable<CustomerInfo>> GetInactiveMemberListAsync(GetInactiveMemberListParams input)
    {
        var para = new DynamicParameters();
        para.Add("_SiteId", input.SiteId);
        para.Add("_Days", input.Days);

        return _reportDbClient.QueryAsync<CustomerInfo>("Store_InactiveCustList_Get", para);;
    }
}