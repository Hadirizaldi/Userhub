namespace UserHub.Application.Abstractions.Auth;

public interface ICurrentUserAccessor
{
    int UserId { get; }
    bool IsAdmin { get; }
}
