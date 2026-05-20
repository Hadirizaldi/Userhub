using System;
using System.Collections.Generic;

namespace UserHub.Infrastructure.Persistence.Entities;

public partial class LoginLogs
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public DateTime LoginAt { get; set; }

    public DateTime? LogoutAt { get; set; }

    public bool? IsLoggedIn { get; set; }

    public virtual Users User { get; set; } = null!;
}
