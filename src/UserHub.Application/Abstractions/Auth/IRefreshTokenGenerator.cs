namespace UserHub.Application.Abstractions.Auth;

public interface IRefreshTokenGenerator
{
    (string plaintext, string hash) Generate();
    string Hash(string plaintext);
}