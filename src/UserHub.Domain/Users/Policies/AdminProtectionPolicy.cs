using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Domain.Users.Policies;

public sealed class AdminProtectionPolicy
{
    public void EnsureNotLastAdmin(int activeAdminCount)
    {
        if(activeAdminCount <= 1)
            throw new ConflictException(
                ErrorCodes.LastAdmin,
                "Cannot permanently delete the last active admin user.");
    }
}