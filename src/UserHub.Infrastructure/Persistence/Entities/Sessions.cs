using System;
using System.Collections.Generic;

namespace UserHub.Infrastructure.Persistence.Entities;

public partial class Sessions
{
    public long Id { get; set; }

    public long LoginLogId { get; set; }

    public int UserId { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual LoginLogs LoginLog { get; set; } = null!;

    public virtual ICollection<RefreshTokens> RefreshTokens { get; set; } = new List<RefreshTokens>();

    public virtual Users User { get; set; } = null!;
}
