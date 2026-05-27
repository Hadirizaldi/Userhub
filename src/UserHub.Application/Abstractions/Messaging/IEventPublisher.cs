namespace UserHub.Application.Abstractions.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(string routingKey, string payload, Guid messageId, CancellationToken ct);
}