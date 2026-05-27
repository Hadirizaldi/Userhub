using System.Text.Json;
using UserHub.Application.Abstractions.Messaging;
using UserHub.Application.Abstractions.Time;
using UserHub.Infrastructure.Persistence;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Messaging;

public sealed class OutboxWriter(AppDbContext db, IClock clock) : IOutboxWriter
{
    public async Task AddAsync<T>(string Type, T payload, CancellationToken cancellationToken) where T : notnull
    {
        db.OutboxMessages.Add(new OutboxMessages
        {
            Id = Guid.NewGuid(),
            Type = Type,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAt = clock.UtcNow,
            Attempts = 0
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}