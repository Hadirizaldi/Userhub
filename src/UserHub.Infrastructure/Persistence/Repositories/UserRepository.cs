using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Users.Queries.GetUsers;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<(IReadOnlyList<UserListItemDto> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken )
    {
        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Trim()}%";
            query = query.Where(u => 
                EF.Functions.ILike(u.Fullname, s) ||
                EF.Functions.ILike(u.Email, s) ||
                EF.Functions.ILike(u.Nip, s)
            );
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListItemDto(
                u.Id,
                u.Nip,
                u.Fullname,
                u.Email,
                u.StatusId,
                u.Status.Name,
                u.ConditionStatusId,
                u.ConditionStatus.Name,
                u.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}