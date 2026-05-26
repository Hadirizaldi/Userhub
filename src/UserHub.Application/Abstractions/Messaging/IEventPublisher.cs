namespace UserHub.Application.Abstractions.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>( string routingKey,T @event, CancellationToken cancellationToken );
}