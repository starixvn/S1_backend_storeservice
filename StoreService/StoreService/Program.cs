using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

using StoreService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();

builder.Configuration.AddJsonFile("appsettings.json",
        optional: true,
        reloadOnChange: false);

var remoteUrls = GetRemoteUrls();
//remoteUrls = builder.Configuration.CheckLocalConfig(builder.Environment, args, remoteUrls);
builder.Configuration.AddRemoteJsonConfig(remoteUrls);

builder.Host.UseSerilog((context, configuration) =>
{
    var excludesMessage = builder.Configuration.GetSection("ExcludesMessage").Get<string[]>();
    var excludesApiEndpoints = builder.Configuration.GetSection("ExcludesApiEndpoint").Get<string[]>();

    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Filter.ByExcluding(c =>
            c.Properties.Any(p =>
                excludesApiEndpoints.Any(item => p.Value.ToString().Contains(item))
                || excludesMessage.Any(item => c.MessageTemplate.ToString().Contains(item))))
        .Enrich.WithExceptionDetails();

    OverrideLoggerWriteToLFile(configuration);
});

builder.Services
    .AddDatabaseConnection(builder.Configuration)
    .RegisterServices()
    .AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.UseRouting();
app.UseRequestLogMiddleware();
app.UseResponseLogMiddleware();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/health", async context =>
    {
        await context.Response.WriteAsync($"Healthy");
    });

    endpoints.MapControllers();
});

SentrySdk.CaptureMessage($"Started result Store Service at {DateTime.Now.ToString()}");
app.Run();
return;

void OverrideLoggerWriteToLFile(LoggerConfiguration configuration)
{
    const string cashSiteBrand = "Nova";

    var brand = Environment.GetEnvironmentVariable("Brand") ?? string.Empty;
    if (brand != cashSiteBrand) return;

    var logFilePath = $"/storeservice/storeservice-{DateTime.Now:yyyyMMddHHmmssfff}.log";
    configuration
        .WriteTo.File(new JsonFormatter(),
            logFilePath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 60,
            fileSizeLimitBytes: 10485760,
            rollOnFileSizeLimit: true);
}

string[] GetRemoteUrls()
{
    return builder.Configuration.GetSection("ConfigService").Get<string[]>();
}