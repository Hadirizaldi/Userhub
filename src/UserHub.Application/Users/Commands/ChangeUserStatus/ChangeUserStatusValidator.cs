using FluentValidation;

namespace UserHub.Application.Users.Commands.ChangeUserStatus;

public sealed class ChangeUserStatusValidator : AbstractValidator<ChangeUserStatusRequest>
{
    public ChangeUserStatusValidator()
    {
        RuleFor(x => x.StatusId).GreaterThan(0);
    }
}