using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NSubstitute;

using StoreHangfireService.Extensions;

using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;

namespace StoreHangfireService.Test;

internal class DepositRewardServiceTest
{
    private readonly IServiceProvider _provider;

    private readonly MySqlClient _dbClient;

    public DepositRewardServiceTest()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            EnvironmentName = "Local",
        });

        var remoteUrls = builder.Configuration.GetSection("ConfigService").Get<string[]>();
        remoteUrls = builder.Configuration.CheckLocalConfig(builder.Environment, new[] { "Nova" }, remoteUrls);
        builder.Configuration.AddRemoteJsonConfig(remoteUrls);

        var logger = Substitute.For<ILogger<MySqlClient>>();
        builder.Services.AddTransient(p => logger);
        builder.Services
            .AddDatabaseConnection(builder.Configuration)
            .RegisterServices(builder.Configuration)
            .AddHangfireService(builder.Configuration)
            .RegisterScheduledJobs(builder.Configuration);

        var app = builder.Build();

        _dbClient = app.Services.GetServices<MySqlClient>().First(p => p.DataBase == DataBaseEnum.Main);
        _provider = app.Services;
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}
