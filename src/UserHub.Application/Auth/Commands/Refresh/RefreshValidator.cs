using FluentValidation;

namespace UserHub.Application.Auth.Commands.Refresh;

public sealed class RefreshValidator : AbstractValidator<RefreshRequest>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(200);
    }
}