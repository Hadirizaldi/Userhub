using FluentValidation;

namespace UserHub.Application.Users.Queries.GetUsers;

public sealed class GetUsersValidator : AbstractValidator<GetUsersRequest>
{
    public GetUsersValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(100);
    }
}
