namespace UserHub.Application.AuditLogs.Queries.GetAuditLogs;

public sealed record AuditLogDto(
    long Id,
    int? ActorUserId,
    string? ActorName,
    string? ActorEmail,
    string Action,
    string? EntityType,
    int? EntityId,
    string? Changes,
    string? IpAddress,
    DateTime CreatedAt
);