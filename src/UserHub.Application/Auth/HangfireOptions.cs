using System.ComponentModel.DataAnnotations;

namespace UserHub.Application.Auth;

public sealed class HangfireOptions
{
    public const string SectionName = "Hangfire";

    [Required, MinLength(1)]
    public string SchemaName { get; set; } = "hangfire";

    [Range(1, 64)]
    public int WorkerCount { get; set; } = 4;

    [Range(1, 300)]
    public int PollingIntervalSeconds { get; set; } = 15;

    public bool PrepareSchemaIfNecessary { get; set; } = true;
}
