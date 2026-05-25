using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.AuditLogs;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IAuditLogger auditLogger,
    IClock clock)
{
    public async Task HandleAsync(
        int id,
        DeleteUserRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Force == true)
        {
            await HandleHardDeleteAsync(id, cancellationToken);
        }

        await HandleSoftDeleteAsync(id, cancellationToken);
    }

    private async Task HandleSoftDeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var success = await userRepository.SoftDeleteAsync( id, clock.UtcNow, cancellationToken);
        if (!success) throw NotFoundException.For("User", id);

        await sessionRepository.RevokeAllForUserAsync( id, clock.UtcNow, cancellationToken);
        
        await auditLogger.LogAsync(
            new AuditEntry("user.soft_delete", "user", id),
            cancellationToken
        );
    }

    private async Task HandleHardDeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var info = await userRepository.GetForHardDeleteAsync(id, cancellationToken);
        if (info is null) throw NotFoundException.For("User", id);

        await userRepository.HardDeleteAsync(id, cancellationToken);

        await auditLogger.LogAsync(
            new AuditEntry("user.hard_delete", "user", id),
            cancellationToken
        );
    }
}
