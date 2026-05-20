using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Users.Queries.GetUsers;

public sealed record GetUsersRequest : PagedQuery
{
    public string? Search { get; init; }
}
