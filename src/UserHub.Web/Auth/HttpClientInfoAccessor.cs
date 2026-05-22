using Microsoft.AspNetCore.HttpOverrides;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Auth;

namespace UserHub.Web.Auth;

public sealed class HttpClientInfoAccessor(IHttpContextAccessor accessor) : IClientInfoAccessor
{
    public ClientInfo Current
    {
        get
        {
            var ctx = accessor.HttpContext;
            if (ctx is null) return new ClientInfo(null, null);

            var ip = ctx.Connection.RemoteIpAddress?.ToString();
            var ua = ctx.Request.Headers.UserAgent.ToString();

            return new ClientInfo(
                string.IsNullOrWhiteSpace(ip) ? null : ip,
                string.IsNullOrWhiteSpace(ua) ? null : ua);
        }
    }

}