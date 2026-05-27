using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Messaging;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Messaging.Jobs;
using UserHub.Infrastructure.Persistence;

namespace UserHub.Infrastructure.Messaging;

public sealed class OutboxReader(AppDbContext db, IClock clock) : IOutboxReader
{
    public async Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken) =>
        await db.OutboxMessages
            .AsNoTracking()
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .Select(m => new OutboxMessage(
                m.Id,
                m.Type,
                m.Payload
            ))
            .ToListAsync(cancellationToken);

    public async Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken) =>
        await db.OutboxMessages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.ProcessedAt, clock.UtcNow),
                cancellationToken);

    public async Task MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken) =>
        await db.OutboxMessages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Attempts, m => m.Attempts + 1)
                .SetProperty(m => m.Error, error),
                cancellationToken);
}