using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.AuditLogs;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Roles;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IAuditLogger auditLogger,
    ICurrentUserAccessor currentUser,
    UserDeletionPolicy userDeletionPolicy,
    IClock clock)
{
    public async Task HandleAsync(
        int id,
        DeleteUserRequest request,
        CancellationToken cancellationToken)
    {
        userDeletionPolicy.EnsureNotSelf(currentUser.UserId, id);

        if (request.Force == true)
        {
            await HandleHardDeleteAsync(id, cancellationToken);
            return;
        }

        await HandleSoftDeleteAsync(id, cancellationToken);
    }

    private async Task HandleSoftDeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var info = await userRepository.GetForDeletionAsync(id, cancellationToken);
        if (info is null) throw NotFoundException.For("User", id);

        await EnsureNotLastAdminAsync(info, cancellationToken);

        var success = await userRepository.SoftDeleteAsync(id, clock.UtcNow, cancellationToken);
        if (!success) throw NotFoundException.For("User", id);

        await sessionRepository.RevokeAllForUserAsync(id, clock.UtcNow, cancellationToken);

        await auditLogger.LogAsync(
            new AuditEntry("user.soft_delete", "user", id),
            cancellationToken);
    }

    private async Task HandleHardDeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var info = await userRepository.GetForDeletionAsync(id, cancellationToken);
        if (info is null) throw NotFoundException.For("User", id);

        await EnsureNotLastAdminAsync(info, cancellationToken);

        await userRepository.HardDeleteAsync(id, cancellationToken);

        await auditLogger.LogAsync(
            new AuditEntry("user.hard_delete", "user", id),
            cancellationToken);
    }

    private async Task EnsureNotLastAdminAsync(
        UserDeletionInfo info,
        CancellationToken cancellationToken)
    {
        if (info.IsDeleted || info.RoleName != RoleNames.Admin) return;

        var activeAdmins = await userRepository.CountActiveByRoleNameAsync(
            RoleNames.Admin, cancellationToken);

        userDeletionPolicy.EnsureNotLastAdmin(activeAdmins);
    }
}
