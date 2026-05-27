using UserHub.Application.Email;

namespace UserHub.Application.Abstractions.Email;

public interface IEmailSender
{
    Task SendAsync( EmailMessage message, CancellationToken cancellationToken );
}