using UserHub.Domain.Common.Exceptions;

namespace UserHub.Domain.Roles.Policies;

public sealed class RoleProtectionPolicy
{
    public void EnsureMutable(bool isSystem)
    {
        if (isSystem)
        {
            throw new ForbiddenException(
                "ROLE_IS_SYSTEM",
                "System roles cannot be modified or deleted.");
        }
    }
}
