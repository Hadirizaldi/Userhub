using System.ComponentModel.DataAnnotations;

namespace UserHub.Infrastructure.Settings;

public sealed class DatabaseOptions
{
    public const string SectionName = "DbContextSettings";

    [Required]
    public PostgresOptions Postgresql { get; set; } = new();
}