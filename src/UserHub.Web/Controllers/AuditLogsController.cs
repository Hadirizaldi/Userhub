using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserHub.Application.AuditLogs.Queries.GetAuditLogs;
using UserHub.Application.Common.Pagination;
using UserHub.Web.Auth;

namespace UserHub.Web.Controllers;

[ApiController]
[Route("api/v1/audit-logs")]
[Tags("AuditLogs")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public sealed class AuditLogsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] GetAuditLogsRequest request,
        [FromServices] GetAuditLogsService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(request, cancellationToken);
        return Ok(result);
    }
}