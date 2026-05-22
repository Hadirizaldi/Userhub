using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Domain.Roles.Policies;

public sealed class RoleProtectionPolicy
{
    public void EnsureMutable(bool isSystem)
    {
        if (isSystem)
        {
            throw new ForbiddenException(
                ErrorCodes.RoleIsSystem,
                "System roles cannot be modified or deleted.");
        }
    }
}
