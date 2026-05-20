using FluentValidation;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.UpdateUser;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator(PhonePolicy phonePolicy)
    {
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.Phone)
            .Must(p => phonePolicy.IsValid(phonePolicy.Normalize(p)))
            .WithMessage("'Phone' must be in E.164 format (e.g. +6281234567890).");
    }
}