using FluentValidation;

namespace UserHub.Application.Users.Commands.ChangeUserRole;

public sealed class ChangeRoleUserValidator : AbstractValidator<ChangeUserRoleRequest>
{
    public ChangeRoleUserValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0);
    }
}