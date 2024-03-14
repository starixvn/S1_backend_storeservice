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

namespace StoreHangfireService.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<MySqlClient>>();
        var serviceType = Environment.GetEnvironmentVariable("ServiceType") ?? configuration.GetValue<string>("ServiceType");
        return services
            .AddSingleton(param => new MySqlClient(configuration.GetConnectionString($"{serviceType}_MainDB"), logger)
            {
                DataBase = DataBase.Main
            })
            .AddSingleton(param => new MySqlClient(configuration.GetConnectionString($"{serviceType}_ReportDB"), logger)
            {
                DataBase = DataBase.Report
            });
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
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

    public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration)
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

    public static IServiceCollection RegisterScheduledJobs(this IServiceCollection services, IConfiguration configuration)
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
