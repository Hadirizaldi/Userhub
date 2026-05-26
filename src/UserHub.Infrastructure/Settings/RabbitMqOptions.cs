using System.ComponentModel.DataAnnotations;

namespace UserHub.Infrastructure.Settings;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    [Required]
    public string Host { get; set; } = "localhost";

    [Required]
    public int Port { get; set; } = 5672;

    [Required]
    public string Username { get; set; } = "guest";

    [Required]
    public string Password { get; set; } = "guest";

    [Required]
    public string Exchange { get; set; } = "userhub.events";

    [Required]
    public string Queue {get; set;} = "userhub.user-registered";
}