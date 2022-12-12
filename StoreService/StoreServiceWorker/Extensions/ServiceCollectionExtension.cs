using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;
using StoreServiceWorker.Workers;

namespace StoreServiceWorker.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<MySqlClient>>();

        return services
            .AddSingleton(param =>
                new MySqlClient(configuration.GetConnectionString("ConnectionString"), logger)
                {
                    DataBase = DataBaseEnum.Main
                })
            .AddSingleton(param =>
                new MySqlClient(configuration.GetConnectionString("ConnectionStringReport"), logger)
                {
                    DataBase = DataBaseEnum.Report
                });
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<PointSettingBLL>()
            .AddSingleton<RewardPointBLL>()
            .AddSingleton<MemberBLL>()
            .AddSingleton<TurnoverWorkerBLL>()

            .AddSingleton<PointSettingService>()
            .AddSingleton<RewardPointService>()
            .AddSingleton<MemberService>()
            .AddSingleton<TurnoverPeriodService>()
            .AddSingleton<TurnoverWorkerService>()
            .AddSingleton<CommonService>();
    }

    public static IServiceCollection RegisterHostServices(this IServiceCollection services)
    {
        return services
            //.AddHostedService<DepositRewardPointWorker>()
            //.AddHostedService<PointExpiryWorker>()
            //.AddHostedService<TurnoverRewardPointWorker>();
    }
}