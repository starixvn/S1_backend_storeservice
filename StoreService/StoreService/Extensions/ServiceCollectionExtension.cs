using StoreService.Core.Service.BLL;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<MySqlClient>>();
        var serviceType = Environment.GetEnvironmentVariable("ServiceType") ?? configuration.GetValue<string>("ServiceType");

        return services
            .AddSingleton(param =>
                new MySqlClient(configuration.GetConnectionString($"{serviceType}_MainDB"), logger)
                {
                    DataBase = DataBaseEnum.Main
                })
            .AddSingleton(param =>
                new MySqlClient(configuration.GetConnectionString($"{serviceType}_ReportDB"), logger)
                {
                    DataBase = DataBaseEnum.Report
                });
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddMemoryCache()
            .AddSingleton<CacheHelper>()
            //.AddSingleton<ICacheHelper, RedisCacheHelper>()
            .AddSingleton<GiftManagementService>()
            .AddSingleton<GiftManagementBLL>()
            .AddSingleton<RedemptionBLL>()
            .AddSingleton<ReportBLL>()
            .AddSingleton<RedemptionService>()
            .AddSingleton<ReportService>()
            .AddSingleton<PointSettingService>()
            .AddSingleton<PointSettingBLL>()
            .AddSingleton<MemberService>()
            .AddSingleton<MemberBLL>()
            .AddSingleton<RewardPointService>()
            .AddSingleton<RewardPointBLL>()
            .AddSingleton<CommonService>()
            .AddSingleton<CommonBLL>();
    }
}