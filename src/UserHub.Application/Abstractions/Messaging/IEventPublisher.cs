namespace UserHub.Application.Abstractions.Messaging;

public interface IEvenPublisher
{
    Task PublishAsync<T>( string routingKey,T @event, CancellationToken cancellationToken );
}