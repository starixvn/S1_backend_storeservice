using Hangfire;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

using StoreHangfireService.Extensions;
using StoreHangfireService.Extensions.ActionFilters;
using StoreHangfireService.Job;

var builder = WebApplication.CreateBuilder(args);

var remoteUrls = GetRemoteUrls();
builder.Configuration.AddRemoteJsonConfig(remoteUrls);

var excludesMessage = builder.Configuration.GetSection("ExcludesMessage").Get<string[]>();
var excludesApiEndpoints = builder.Configuration.GetSection("ExcludesApiEndpoint").Get<string[]>();

builder.Host.UseSerilog((content, Configuration) =>
{
    Configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Filter.ByExcluding(c =>
            c.Properties.Any(p =>
                excludesApiEndpoints.Any(item => p.Value.ToString().Contains(item))
                || excludesMessage.Any(item => c.MessageTemplate.ToString().Contains(item))
            )
        )
    .Enrich.WithExceptionDetails();

    OverrideLoggerWriteToLFile(Configuration);
});

builder.Services
    .AddDatabaseConnection(builder.Configuration)
    .RegisterServices()
    .RegisterScheduledJobs()
    .AddHangfireService();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/health", () => "OK.");
});

app.UseHangfireDashboard(pathMatch: "", options: new DashboardOptions()
{
    DashboardTitle = "AI.Lab.StoreHangfireService",
    AppPath = null,
    Authorization = new[] { new LocalAuthorizationFilter() }
});

MyJobManager.AddRecurringJob(app.Services);
app.Run();
return;

void OverrideLoggerWriteToLFile(LoggerConfiguration configuration)
{
    const string cashSiteBrand = "Nova";

    var brand = Environment.GetEnvironmentVariable("Brand") ?? string.Empty;
    if (brand != cashSiteBrand) return;

    var logFilePath = $"/storeworker/storehangfireservice-{DateTime.Now:yyyyMMddHHmmssfff}.log";
    configuration
        .WriteTo.File(new JsonFormatter(),
            logFilePath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 60,
            fileSizeLimitBytes: 10485760,
            rollOnFileSizeLimit: true,
            restrictedToMinimumLevel: LogEventLevel.Warning);
}

string[] GetRemoteUrls()
{
    // 如果有 Runtime Environment Variable 優先使用
    // 沒有則使用預設 AppSetting
    return Environment.GetEnvironmentVariable("ConfigServiceUrl") is null
        ? builder.Configuration.GetSection("ConfigService").Get<string[]>()
        : new List<string>() { Environment.GetEnvironmentVariable("ConfigServiceUrl") }.ToArray();
}

