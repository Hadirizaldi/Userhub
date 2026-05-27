using UserHub.Application.Messaging.Jobs;

namespace UserHub.Application.Abstractions.Messaging;

public interface IOutboxReader
{
    Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken);
    Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken);
    Task MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken);
}
