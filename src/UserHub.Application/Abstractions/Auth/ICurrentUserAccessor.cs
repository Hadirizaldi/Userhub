namespace UserHub.Application.Abstractions.Auth;

public interface ICurrentUserAccessor
{
    int UserId { get; }
    int? UserIdOrNull { get; }
    bool IsAdmin { get; }
}
