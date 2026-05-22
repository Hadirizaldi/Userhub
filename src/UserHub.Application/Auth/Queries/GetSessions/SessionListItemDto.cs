namespace UserHub.Application.Auth.Queries.GetSessions;

public sealed record SessionListItemDto(
    long Id,
    string? IpAddress,
    string? UserAgent,
    DateTime LastUsedAt,
    DateTime CreatedAt);
