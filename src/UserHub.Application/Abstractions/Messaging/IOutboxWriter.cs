namespace UserHub.Application.Abstractions.Messaging;

public interface IOutboxWriter
{
    Task AddAsync<T>(string Type, T payload, CancellationToken cancellationToken) where T : notnull;
}