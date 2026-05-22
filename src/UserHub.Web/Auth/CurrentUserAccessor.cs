using System.Security.Claims;
using UserHub.Application.Abstractions.Auth;
using UserHub.Domain.Roles;

namespace UserHub.Web.Auth;

public sealed class CurrentUserAccessor(IHttpContextAccessor accessor) : ICurrentUserAccessor
{
    public int UserId
    {
        get
        {
            var raw = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? accessor.HttpContext?.User.FindFirstValue("sub");

            if (!int.TryParse(raw, out var id))
                throw new InvalidOperationException("User is not authenticated.");

            return id;
        }
    }

    public bool IsAdmin =>
        accessor.HttpContext?.User.IsInRole(RoleNames.Admin) ?? false;
}
