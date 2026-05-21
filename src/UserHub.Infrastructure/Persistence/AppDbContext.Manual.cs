using Microsoft.EntityFrameworkCore;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Persistence;

public partial class AppDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>().HasQueryFilter(u => u.DeletedAt == null);
    }
}
