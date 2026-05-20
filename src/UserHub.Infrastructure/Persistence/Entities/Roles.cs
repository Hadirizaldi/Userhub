using System;
using System.Collections.Generic;

namespace UserHub.Infrastructure.Persistence.Entities;

public partial class Roles
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Users> User { get; set; } = new List<Users>();
}
