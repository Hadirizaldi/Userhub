using UserHub.Application.AuditLogs.Queries.GetAuditLogs;

namespace UserHub.Application.Abstractions.Audit;

public interface IAuditLogReader
{
    Task<(IReadOnlyList<AuditLogDto> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        int? actorUserId,
        string? action,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken
    );
}