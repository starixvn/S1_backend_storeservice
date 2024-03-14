using Hangfire;
using Hangfire.Console.Extensions;
using Hangfire.Dashboard.Management;
using Hangfire.MemoryStorage;

using StackExchange.Redis;

using StoreHangfireService.Job;
using StoreHangfireService.Job.Interfaces;
using StoreHangfireService.Model.Options;
using StoreHangfireService.Service;
using StoreHangfireService.Service.Expired;
using StoreHangfireService.Service.Reward;

using StoreService.Core.Service.BLL;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;

using static Org.BouncyCastle.Math.EC.ECCurve;

namespace StoreHangfireService.Extensions;

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
            //StoreHangfireService
            .AddSingleton<ConfigService>()

            .AddSingleton<PointExpiryService>()
            .AddSingleton<DepositRewardService>()
            .AddSingleton<TurnoverRewardService>()

            //Store.Core
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

    public static IServiceCollection AddHangfireService(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var jobGroups = provider.GetRequiredService<Dictionary<string, List<JobOption>>>();

        return services
        .AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseColouredConsoleLogProvider()
                .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.RecurringJobCount)
                .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.RetriesCount)
                .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.ProcessingCount)
                .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.SucceededCount)
                .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.FailedCount)
                .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.AwaitingCount)
                .UseManagementPages(p => p.AddJobs(()
                    => typeof(IScheduledJob).Assembly.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IScheduledJob))).ToArray()))
                .UseSerilogLogProvider()
                .UseMemoryStorage();
        })
        .AddHangfireConsoleExtensions()
        .AddHangfireServer(config =>
        {
            config.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            var queues = jobGroups.Select(item => item.Key.ToLower()).ToList();
            queues.Add("default");
            config.Queues = queues.ToArray();
            config.WorkerCount = Environment.ProcessorCount * 10;
        });
    }

    public static IServiceCollection RegisterScheduledJobs(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var configService = provider.GetRequiredService<ConfigService>();
        var jobGroups = configService.Query<JobOption>("Service_ScheduledJob_Get", new { Project = "Store" })
           .GroupBy(i => i.Group)
           .ToDictionary(i => i.Key, i => i.ToList());
        services.AddSingleton(provider => jobGroups);
        MyJobManager.SettingJob(jobGroups);

        var scheduledJobs = typeof(IScheduledJob).Assembly.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IScheduledJob)));
        foreach (var job in scheduledJobs)
        {
            services.AddSingleton(provider => (IScheduledJob)Activator.CreateInstance(job, new object[] { provider }));
        }

        return services;
    }
}
