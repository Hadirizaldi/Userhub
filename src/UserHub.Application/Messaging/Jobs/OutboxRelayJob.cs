using Microsoft.Extensions.Logging;
using UserHub.Application.Abstractions.Messaging;

namespace UserHub.Application.Messaging.Jobs;

public sealed class OutboxRelayJob (
    IOutboxReader outboxReader,
    IEventPublisher publisher,
    ILogger<OutboxRelayJob> logger
)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var pending = await outboxReader.GetPendingAsync(batchSize: 20, cancellationToken);

        foreach (var msg in pending)
        {
            try
            {
                await publisher.PublishAsync(msg.Type, msg.Payload, msg.Id, cancellationToken);
                await outboxReader.MarkProcessedAsync(msg.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Relay failed to publish outbox {Id}, left pending", msg.Id);
                await outboxReader.MarkFailedAsync(msg.Id, ex.Message, cancellationToken);
            }
        }
    }
}