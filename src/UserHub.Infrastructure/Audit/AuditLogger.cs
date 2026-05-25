using System.Text.Json;
using Microsoft.Extensions.Logging;
using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.AuditLogs;
using UserHub.Infrastructure.Persistence;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Audit;

public sealed class AuditLogger(
    AppDbContext db,
    ICurrentUserAccessor currentUser,
    IClientInfoAccessor clientInfo,
    IClock clock,
    ILogger<AuditLogger> logger) : IAuditLogger
{
    public async Task LogAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        try
        {
            db.AuditLogs.Add(new AuditLogs
            {
                ActorUserId = currentUser.UserIdOrNull,
                Action = entry.Action,
                EntityType = entry.EntityType,
                EntityId = entry.EntityId,
                Changes = entry.Changes is null ? null : JsonSerializer.Serialize(entry.Changes),
                IpAddress = clientInfo.Current.IpAddress,
                CreatedAt = clock.UtcNow
            });

        await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to write audit log for action {Action}", entry.Action);
        }
    }

}