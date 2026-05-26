namespace UserHub.Application.Users.Events;

public sealed record UserRegisteredEvent(
    int UserId,
    string Email,
    string Fullname
)
{
    public const string RoutingKey = "user.registered";
}