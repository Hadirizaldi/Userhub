namespace UserHub.Domain.Users.Policies;

public sealed class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 100;

    public bool IsStrong(string password) =>
        !string.IsNullOrEmpty(password)
        && password.Length >= MinLength
        && password.Length <= MaxLength
        && password.Any(char.IsUpper)
        && password.Any(char.IsLower)
        && password.Any(char.IsDigit);
}