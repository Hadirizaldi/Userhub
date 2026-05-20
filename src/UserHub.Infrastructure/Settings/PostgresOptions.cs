using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace UserHub.Infrastructure.Settings;


public sealed class PostgresOptions
{
    [Required, MinLength(1)]
    [ConfigurationKeyName("ConnectionString_core")]
    public string Core { get; init; } = string.Empty;
}