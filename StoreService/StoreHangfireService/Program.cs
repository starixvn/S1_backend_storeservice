using Hangfire;

using Serilog;
using Serilog.Exceptions;

using StoreHangfireService.Extensions;
using StoreHangfireService.Extensions.ActionFilters;
using StoreHangfireService.Job;

var builder = WebApplication.CreateBuilder(args);

var remoteUrls = builder.Configuration.GetSection("ConfigService").Get<string[]>();
remoteUrls = builder.Configuration.CheckLocalConfig(builder.Environment, args, remoteUrls);
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
});

builder.Services
    .AddDatabaseConnection(builder.Configuration)
    .RegisterServices(builder.Configuration)
    .RegisterScheduledJobs(builder.Configuration)
    .AddHangfireService(builder.Configuration);

var app = builder.Build();

if (!app.Environment.EnvironmentName.Equals("Local"))
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
