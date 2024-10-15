using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

using StoreService.Extensions;

var builder = WebApplication.CreateBuilder(args);

var remoteUrls = GetRemoteUrls();
remoteUrls = builder.Configuration.CheckLocalConfig(builder.Environment, args, remoteUrls);
builder.Configuration.AddRemoteJsonConfig(remoteUrls);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Store Service",
        Version = "v1",
        Description = "API documentation for Store Service"
    });
});

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

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseRequestLogMiddleware();
app.UseResponseLogMiddleware();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/health", async context =>
    {
        await context.Response.WriteAsync($"OK");
    });
    endpoints.MapControllers();
});

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
    // 如果有 Runtime Environment Variable 優先使用
    // 沒有則使用預設 AppSetting
    return Environment.GetEnvironmentVariable("ConfigServiceUrl") is null
        ? builder.Configuration.GetSection("ConfigService").Get<string[]>()
        : new List<string>() { Environment.GetEnvironmentVariable("ConfigServiceUrl") }.ToArray();
}