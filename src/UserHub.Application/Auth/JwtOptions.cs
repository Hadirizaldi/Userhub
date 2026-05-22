using System.ComponentModel.DataAnnotations;

namespace UserHub.Application.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required, MinLength(32)]
    public string Secret { get; set; } = string.Empty;
    [Required]
    public string Issuer { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Range(1, 1440)]
    public int AccessTokenMinutes { get; set; }
    [Range(1,90)]
    public int RefreshTokenDays { get; set; }
}