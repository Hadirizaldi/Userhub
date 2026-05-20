using MediatR;
using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Users.Queries.GetUsers;

public sealed record GetUserQuery : PagedQuery, IRequest<PagedResult<UserListItemDto>>
{
    public string? Search { get; set; }
}