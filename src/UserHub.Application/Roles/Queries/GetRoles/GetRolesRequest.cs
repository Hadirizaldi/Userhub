using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Roles.Queries.GetRoles;

public sealed record GetRolesRequest : PagedQuery
{
    public string? Search { get; init; }
}
