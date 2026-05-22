using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Domain.Users.Policies;

public sealed class RoleChangePolicy
{
    public void EnsureCanChange(int currentStatusId, int activeStatusId)
    {
        if (currentStatusId != activeStatusId)
            throw new ForbiddenException(ErrorCodes.UserNotActive, "Role can only be changed for active users.");
    }
}
