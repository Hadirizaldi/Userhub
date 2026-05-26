using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Domain.Users.Policies;

public sealed class UserDeletionPolicy
{
    public void EnsureNotLastAdmin(int activeAdminCount)
    {
        if(activeAdminCount <= 1)
            throw new ConflictException(
                ErrorCodes.LastAdmin,
                "Cannot delete the last active admin user.");
    }

    public void EnsureNotSelf(int actorId, int targetId)
    {
        if (actorId == targetId)
            throw new ForbiddenException(
                ErrorCodes.CannotDeleteSelf,
                "You cannot delete your own account."
            );
    }
}