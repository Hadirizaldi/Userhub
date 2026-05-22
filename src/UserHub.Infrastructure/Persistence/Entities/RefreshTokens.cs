using System;
using System.Collections.Generic;

namespace UserHub.Infrastructure.Persistence.Entities;

public partial class RefreshTokens
{
    public long Id { get; set; }

    public long SessionId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Sessions Session { get; set; } = null!;
}
