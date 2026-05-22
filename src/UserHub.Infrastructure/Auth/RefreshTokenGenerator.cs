using System.Security.Cryptography;
using UserHub.Application.Abstractions.Auth;

namespace UserHub.Infrastructure.Auth;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public (string plaintext, string hash) Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(48);
        var plaintext = Convert.ToBase64String(bytes)
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');  

        return (plaintext, Hash(plaintext));
    }

    public string Hash(string plaintext)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(plaintext));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}