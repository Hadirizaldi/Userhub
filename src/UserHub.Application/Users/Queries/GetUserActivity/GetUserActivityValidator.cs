using FluentValidation;

namespace UserHub.Application.Users.Queries.GetUserActivity;

public sealed class GetUserActivityValidator : AbstractValidator<GetUserActivityRequest>
{
    public GetUserActivityValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(100);
    }
}