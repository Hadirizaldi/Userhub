using FluentValidation;

namespace UserHub.Application.Auth.Commands.Logout;

public sealed class LogoutValidator : AbstractValidator<LogoutRequest>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(200);
    }
}