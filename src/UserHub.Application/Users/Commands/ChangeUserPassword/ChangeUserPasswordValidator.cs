using FluentValidation;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.ChangeUserPassword;

public sealed class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordRequest>
{
    public ChangeUserPasswordValidator(PasswordPolicy passwordPolicy)
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Must(passwordPolicy.IsStrong)
            .WithMessage("'NewPassword' must be 8-128 chars and contain uppercase, lowercase, and digit.");
    }
}
