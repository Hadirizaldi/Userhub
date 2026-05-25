using UserHub.Application.AuditLogs;

namespace UserHub.Application.Abstractions.Audit;

public interface IAuditLogger
{
    Task LogAsync(AuditEntry entry, CancellationToken cancellationToken);
}