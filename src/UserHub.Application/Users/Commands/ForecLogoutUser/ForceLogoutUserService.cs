using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.AuditLogs;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Commands.ForceLogoutUser;

public sealed class ForceLogoutUserService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IClock clock,
    IAuditLogger auditLogger
)
{
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var statusId = await userRepository.GetStatusIdAsync(id, cancellationToken);
        if (statusId is null) throw NotFoundException.For("User", id);

        await sessionRepository.RevokeAllForUserAsync(id, clock.UtcNow, cancellationToken);

        await auditLogger.LogAsync(
            new AuditEntry("user.force_logout", "user", id), cancellationToken);
    }
}