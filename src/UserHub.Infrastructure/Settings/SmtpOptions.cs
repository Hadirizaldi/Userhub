using System.ComponentModel.DataAnnotations;

namespace UserHub.Infrastructure.Settings;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";
    
    [Required]
    public string Host { get; init; } = default!;

    [Range(1, 65535)]
    public int Port { get; init; }

    [Required, EmailAddress]
    public string FromAddress { get; init; } = default!;

    [Required]
    public string FromName { get; init; } = default!;
}