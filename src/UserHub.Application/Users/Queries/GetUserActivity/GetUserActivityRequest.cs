using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Users.Queries.GetUserActivity;

public sealed record GetUserActivityRequest : PagedQuery
{
    public string? Search { get; init; }
    public bool? IsLoggedIn { get; init; }
}