using StoreService.Extensions;

namespace StoreService;

public class Startup
{
    private readonly IConfiguration _configuration;
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDatabaseConnection(_configuration)
            .RegisterServices(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseRequestLogMiddleware();
        app.UseResponseLogMiddleware();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/health", async context =>
            {
                await context.Response.WriteAsync($"{env.EnvironmentName}-{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
            });
        });
    }
}