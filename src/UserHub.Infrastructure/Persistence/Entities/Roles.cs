using System;
using System.Collections.Generic;

namespace UserHub.Infrastructure.Persistence.Entities;

public partial class Roles
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsSystem { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Users> User { get; set; } = new List<Users>();
}
