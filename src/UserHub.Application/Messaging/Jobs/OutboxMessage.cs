namespace UserHub.Application.Messaging.Jobs;

public sealed record OutboxMessage(
    Guid Id,
    string Type,
    string Payload
);