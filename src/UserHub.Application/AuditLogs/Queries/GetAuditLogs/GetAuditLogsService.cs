using FluentValidation;
using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Common.Pagination;

namespace UserHub.Application.AuditLogs.Queries.GetAuditLogs;

public sealed class GetAuditLogsService (
    IValidator<GetAuditLogsRequest> validator,
    IAuditLogReader reader
)
{
    public async Task<PagedResult<AuditLogDto>> HandleAsync(
        GetAuditLogsRequest request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var (items, total) = await reader.ListAsync(
            request.Page, 
            request.PageSize,
            request.ActorUserId, 
            request.Action,
            request.FromUtc, 
            request.ToUtc,
            cancellationToken
        );

        return new PagedResult<AuditLogDto>(items, request.Page, request.PageSize, total);
    }
}