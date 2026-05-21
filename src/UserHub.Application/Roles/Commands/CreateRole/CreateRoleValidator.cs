using FluentValidation;

namespace UserHub.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 50);
    }
}
