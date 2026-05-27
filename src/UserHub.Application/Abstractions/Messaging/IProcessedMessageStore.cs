namespace UserHub.Application.Abstractions.Messaging;

public interface IProcessedMessageStore
{
    Task<bool> ExistsAsync(Guid messageId, CancellationToken ct);
    Task AddAsync(Guid messageId, CancellationToken ct);
}