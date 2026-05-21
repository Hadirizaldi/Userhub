namespace UserHub.Application.Users.Commands.ChangeUserPassword;

public sealed record ChangeUserPasswordData(string PasswordHash, DateTime UpdatedAt);
