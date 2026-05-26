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
            $"Halo {@event.Fullname},\n\n" +
            "Akun kamu di UserHub berhasil dibuat.\n" +
            "(Verifikasi email menyusul di tahap berikutnya.)\n";

        await emailSender.SendAsync(
            new Email.EmailMessage(
                @event.Email,
                "Selamat datang di UserHub!",
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