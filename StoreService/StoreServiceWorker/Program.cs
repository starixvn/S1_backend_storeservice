using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using StoreServiceWorker.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddRemoteJsonConfig(builder.Configuration.GetSection("ConfigService").Get<string[]>());

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails();
}).ConfigureServices((hostContext, services) =>
{
    services.RegisterHostServices();
});

builder.Services
    .AddDatabaseConnection(builder.Configuration)
    .RegisterServices(builder.Configuration);

var app = builder.Build();

app.Run();