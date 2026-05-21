using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Roles.Commands.CreateRole;
using UserHub.Application.Roles.Commands.UpdateRole;
using UserHub.Application.Roles.Queries.GetRoleById;
using UserHub.Application.Roles.Queries.GetRoles;
using UserHub.Application.Roles.Queries.LookupRoles;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(AppDbContext db) : IRoleRepository
{
    public async Task<IReadOnlyList<RoleLookupDto>> GetLookupAsync(CancellationToken cancellationToken)
    {
        return await db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new RoleLookupDto(r.Id, r.Name))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        db.Roles.AnyAsync(r => r.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken) =>
        db.Roles.AnyAsync(r => r.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<IReadOnlyList<int>> GetExistingIdsAsync(
        IReadOnlyList<int> ids, CancellationToken cancellationToken)
    {
        return await db.Roles
            .AsNoTracking()
            .Where(r => ids.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<RoleListItemDto> Items, int TotalCount)> ListAsync(
        int page, int pageSize, string? search, CancellationToken cancellationToken)
    {
        var query = db.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Trim()}%";
            query = query.Where(r => EF.Functions.ILike(r.Name, s));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RoleListItemDto(
                r.Id,
                r.Name,
                r.IsSystem,
                r.User.Count(u => u.DeletedAt == null),
                r.CreatedAt,
                r.UpdatedAt))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.Roles
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RoleDto(r.Id, r.Name, r.IsSystem, r.CreatedAt, r.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<int> AddAsync(CreateRoleData data, CancellationToken cancellationToken)
    {
        var entity = new Roles
        {
            Name = data.Name,
            IsSystem = false,
            CreatedAt = data.CreatedAt,
            UpdatedAt = data.UpdatedAt
        };

        db.Roles.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, UpdateRoleData data, CancellationToken cancellationToken)
    {
        var entity = await db.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (entity is null) return false;

        entity.Name = data.Name;
        entity.UpdatedAt = data.UpdatedAt;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id, DateTime utcNow, CancellationToken cancellationToken)
    {
        var entity = await db.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (entity is null) return false;

        entity.DeletedAt = utcNow;
        entity.UpdatedAt = utcNow;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
