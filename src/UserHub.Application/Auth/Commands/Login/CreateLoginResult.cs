namespace UserHub.Application.Auth.Commands.Login;

public sealed record CreateLoginResult(
    long SessionId,
    long LoginLogId,
    long RefreshTokenId);
