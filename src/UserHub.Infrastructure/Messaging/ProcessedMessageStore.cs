using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Messaging;
using UserHub.Application.Abstractions.Time;
using UserHub.Infrastructure.Persistence;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Messaging;

public sealed class ProcessedMessageStore(AppDbContext db, IClock clock) : IProcessedMessageStore
{
    public Task<bool> ExistsAsync(Guid messageId, CancellationToken ct)
        => db.ProcessedMessages.AsNoTracking().AnyAsync(m => m.Id == messageId, ct);

    public async Task AddAsync(Guid messageId, CancellationToken ct)
    {
        db.ProcessedMessages.Add(new ProcessedMessages { Id = messageId, ProcessedAt = clock.UtcNow });
        await db.SaveChangesAsync(ct);
    }
}