using Microsoft.Extensions.Logging;

namespace UserHub.Application.Users.Events;

public sealed class UserRegisteredHandler(ILogger<UserRegisteredHandler> logger)
{
    public Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken)
    {

        logger.LogInformation(
            "User registered: Id={UserId}, Email={Email}, Fullname={Fullname}",
            @event.UserId,
            @event.Email,
            @event.Fullname
        );
        return Task.CompletedTask;
    }
}