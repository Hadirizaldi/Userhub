using Microsoft.Extensions.Logging;
using UserHub.Application.Abstractions.Email;

namespace UserHub.Application.Users.Events;

public sealed class UserRegisteredHandler(
    ILogger<UserRegisteredHandler> logger,
    IEmailSender emailSender)
{
    public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken)
    {

        var body =
            $"Hi {@event.Fullname},\n\n" +
            "Your UserHub account has been created successfully.\n" +
            "(Email verification will follow in a later phase.)\n";

        await emailSender.SendAsync(
            new Email.EmailMessage(
                @event.Email,
                "Welcome to UserHub!",
                body
            ), cancellationToken
        );

        logger.LogInformation(
            "User registered: Id={UserId}, Email={Email}, Fullname={Fullname}",
            @event.UserId,
            @event.Email,
            @event.Fullname
        );
    }
}