using Serilog;
using Serilog.Exceptions;
using StoreService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddRemoteJsonConfig(builder.Configuration.GetSection("ConfigService").Get<string[]>());

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails();
});

builder.Services
    .AddDatabaseConnection(builder.Configuration)
    .RegisterServices(builder.Configuration)
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
        await context.Response.WriteAsync($"OK");
    });
    endpoints.MapControllers();
});

app.Run();
