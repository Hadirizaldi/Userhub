using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Audit;
using UserHub.Application.AuditLogs.Queries.GetAuditLogs;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class AuditLogReader(AppDbContext db) : IAuditLogReader
{
    public async Task<(IReadOnlyList<AuditLogDto> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        int? actorUserId,
        string? action,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken
    )
    {
        var query = db.AuditLogs.AsNoTracking();

        if (actorUserId.HasValue)
            query = query.Where(a => a.ActorUserId == actorUserId.Value);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action);

        if (fromUtc.HasValue)
            query = query.Where(a => a.CreatedAt >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(a => a.CreatedAt <= toUtc.Value);

        var total = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogDto(
                a.Id,
                a.ActorUserId,
                a.Action,
                a.EntityType,
                a.EntityId,
                a.Changes,
                a.IpAddress,
                a.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}