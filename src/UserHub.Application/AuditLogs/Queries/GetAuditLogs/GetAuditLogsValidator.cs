using FluentValidation;

namespace UserHub.Application.AuditLogs.Queries.GetAuditLogs;

public sealed class GetAuditLogsValidator : AbstractValidator<GetAuditLogsRequest>
{
    public GetAuditLogsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Action).MaximumLength(50);
        RuleFor(x => x.ToUtc)
            .GreaterThanOrEqualTo(x => x.FromUtc!.Value)
            .When(x => x.FromUtc.HasValue && x.ToUtc.HasValue);
    }
}