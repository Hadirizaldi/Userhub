using FluentValidation;

namespace UserHub.Application.Roles.Queries.GetRoles;

public sealed class GetRolesValidator : AbstractValidator<GetRolesRequest>
{
    public GetRolesValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(100);
    }
}
