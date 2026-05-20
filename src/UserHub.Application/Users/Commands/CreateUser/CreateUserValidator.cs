using FluentValidation;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator(PasswordPolicy passwordPolicy, PhonePolicy phonePolicy)
    {
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(passwordPolicy.IsStrong)
            .WithMessage("'Password' must be 8-128 chars and contain uppercase, lowercase, and digit.");

        RuleFor(x => x.Phone)
            .Must(p => phonePolicy.IsValid(phonePolicy.Normalize(p)))
            .WithMessage("'Phone' must be in E.164 format (e.g. +6281234567890).");
    }
}
