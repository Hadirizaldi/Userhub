using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Roles.Queries.GetRoles;

public sealed class GetRolesService(
    IValidator<GetRolesRequest> validator,
    IRoleRepository roleRepository)
{
    public async Task<PagedResult<RoleListItemDto>> HandleAsync(
        GetRolesRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var (items, total) = await roleRepository.ListAsync(
            request.Page, request.PageSize, request.Search, cancellationToken);

        return new PagedResult<RoleListItemDto>(items, request.Page, request.PageSize, total);
    }
}
