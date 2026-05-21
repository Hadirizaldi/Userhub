using FluentValidation;

namespace UserHub.Application.Roles.Commands.UpdateRole;

public sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 50);
    }
}
