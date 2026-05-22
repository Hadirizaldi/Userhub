namespace UserHub.Application.Abstractions.Auth;

public interface IJwtService
{
    string GenerateAccessToken(int userId, string email, string? roleName);
}