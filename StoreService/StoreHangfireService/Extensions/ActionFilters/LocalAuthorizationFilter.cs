using Hangfire.Dashboard;

namespace StoreHangfireService.Extensions.ActionFilters;

public class LocalAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
