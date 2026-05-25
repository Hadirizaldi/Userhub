namespace UserHub.Application.AuditLogs;

public sealed record AuditEntry(
    string Action,
    string? EntityType = null,
    int? EntityId = null,
    object? Changes = null
);