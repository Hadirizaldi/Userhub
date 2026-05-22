namespace UserHub.Application.Auth;

public sealed record ClientInfo
(
    string? IpAddress,
    string? UserAgent
);