using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using UserHub.Application.Abstractions.Email;
using UserHub.Application.Email;
using UserHub.Infrastructure.Settings;

namespace UserHub.Infrastructure.Email;

public sealed class SmtpEmailSender (
    IOptions<SmtpOptions> options
) : IEmailSender
{
    private readonly SmtpOptions _opt = options.Value;

    public async Task SendAsync( EmailMessage message, CancellationToken cancellationToken)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_opt.FromName, _opt.FromAddress));
        mime.To.Add(MailboxAddress.Parse(message.To));
        mime.Subject = message.Subject;
        mime.Body = new TextPart("plain") { Text = message.Body };

        using var client = new SmtpClient();
        // Mailhog tanpa TLS, tanpa AUTH
        await client.ConnectAsync(_opt.Host, _opt.Port, SecureSocketOptions.None, cancellationToken);
        await client.SendAsync(mime, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken: cancellationToken);
    }

}