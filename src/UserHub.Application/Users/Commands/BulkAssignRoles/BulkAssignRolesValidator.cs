using FluentValidation;

namespace UserHub.Application.Users.Commands.BulkAssignRoles;

public sealed class BulkAssignRolesValidator : AbstractValidator<BulkAssignRolesRequest>
{
    public BulkAssignRolesValidator()
    {
        RuleFor(x => x.Assignments)
            .NotEmpty()
            .Must(a => a.Count <= 500)
            .WithMessage("'Assignments' must contain at most 500 items.")
            .Must(a => a.Select(x => x.UserId).Distinct().Count() == a.Count)
            .WithMessage("'Assignments' contains duplicate userId entries.");

        RuleForEach(x => x.Assignments).ChildRules(c =>
        {
            c.RuleFor(a => a.UserId).GreaterThan(0);
            c.RuleFor(a => a.RoleId).GreaterThan(0);
        });
    }
}
