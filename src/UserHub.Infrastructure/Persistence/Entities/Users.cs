using System;
using System.Collections.Generic;

namespace UserHub.Infrastructure.Persistence.Entities;

public partial class Users
{
    public int Id { get; set; }

    public string Nip { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int StatusId { get; set; }

    public int ConditionStatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Phone { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ConditionStatuses ConditionStatus { get; set; } = null!;

    public virtual ICollection<LoginLogs> LoginLogs { get; set; } = new List<LoginLogs>();

    public virtual UserStatuses Status { get; set; } = null!;

    public virtual ICollection<Roles> Role { get; set; } = new List<Roles>();
}
