using System.Net;
using Hangfire.Dashboard;

namespace UserHub.Web.Auth;

public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        var ip = http.Connection.RemoteIpAddress;

        return ip != null && (IPAddress.IsLoopback(ip) || ip.Equals(IPAddress.IPv6Loopback));
    }
}
