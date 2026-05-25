using UserHub.Application.Common.Pagination;

namespace UserHub.Application.AuditLogs.Queries.GetAuditLogs;

public sealed record GetAuditLogsRequest : PagedQuery
{
    public int? ActorUserId { get; init; }
    public string? Action { get; init; }
    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }
}
