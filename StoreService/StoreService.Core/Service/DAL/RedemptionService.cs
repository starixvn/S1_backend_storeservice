using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using StoreService.Core.Service.Library;
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
}
