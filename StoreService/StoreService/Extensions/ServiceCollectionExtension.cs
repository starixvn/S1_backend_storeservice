using StoreService.Core.Service.BLL;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration config)
    {
        var serviceType = Environment.GetEnvironmentVariable("ServiceType") ?? config.GetValue<string>("ServiceType");
        var reportDBConn = config.GetConnectionString($"{serviceType}_ReportDB");
        var reportBackupDB1 = config.GetConnectionString($"{serviceType}_ReportBackupDB1");
        var reportBackupDB2 = config.GetConnectionString($"{serviceType}_ReportBackupDB2");

        //檢查是否有BackupDB連線字串
        if (!string.IsNullOrEmpty(reportBackupDB1) && !string.IsNullOrEmpty(reportBackupDB2))
        {
            // 設定ReportDB連線順序
            services.AddSingleton(provider => new MySqlClient(provider, DataBase.Report, reportDBConn, new List<string>()
                {
                    reportDBConn,   //1b
                    reportBackupDB1,//1c
                    reportBackupDB2,//1a
                }));

            // 設定ReportBackupDB連線順序
            services.AddSingleton(provider => new MySqlClient(provider, DataBase.ReportBackupDB, reportBackupDB1, new List<string>()
                {
                    reportBackupDB1,//1c
                    reportDBConn,   //1b
                    reportBackupDB2,//1a
                }));
        }
        else
        {
            services.AddSingleton(provider => new MySqlClient(provider, DataBase.Report, reportDBConn));
        }

        services.AddSingleton(provider => new MySqlClient(provider, DataBase.Main, config.GetConnectionString($"{serviceType}_MainDB")));

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
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