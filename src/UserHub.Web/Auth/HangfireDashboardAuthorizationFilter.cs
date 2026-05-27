using System.Net;
using Hangfire.Dashboard;

namespace UserHub.Web.Auth;

public sealed class HangfireDashboardAuthorizationFilter(bool isDevelopment) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In Development the app typically runs behind Docker port-forwarding, where the
        // remote IP is the bridge gateway (not loopback). Open the dashboard there.
        if (isDevelopment)
            return true;

        var http = context.GetHttpContext();
        var ip = http.Connection.RemoteIpAddress;

        return ip != null && (IPAddress.IsLoopback(ip) || ip.Equals(IPAddress.IPv6Loopback));
    }
}
