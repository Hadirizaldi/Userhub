using System.ComponentModel.DataAnnotations;

namespace UserHub.Application.Auth;

public sealed class CleanupOptions
{
    public const string SectionName = "Cleanup";

    [Range(1, 365)]
    public int RefreshTokenExpiredRetentionDays { get; set; } = 7;

    [Range(1, 365)]
    public int RefreshTokenRevokedRetentionDays { get; set; } = 30;

    [Range(1, 365)]
    public int SessionRevokedRetentionDays { get; set; } = 90;

    [Required, MinLength(1)]
    public string CronExpression { get; set; } = "0 3 * * *";

    [Required, MinLength(1)]
    public string TimeZone { get; set; } = "UTC";
}
