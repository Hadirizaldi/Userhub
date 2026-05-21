namespace UserHub.Application.Users.Commands.ChangeUserStatus;

public sealed record ChangeUserStatusData(
    int StatusId,
    DateTime UpdatedAt
); 