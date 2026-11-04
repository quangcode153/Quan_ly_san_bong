using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BTLtruc_quan.WebApi.Controllers;

[ApiController]
[Route("api/audit-logs")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Tra cứu lịch sử nhật ký kiểm toán với bộ lọc và phân trang (QT18 & Mục 1.4)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetLogs([FromQuery] AuditLogFilterRequest filter, CancellationToken cancellationToken)
    {
        var logs = await _auditLogService.GetLogsAsync(filter, cancellationToken);
        return Ok(ApiResponse<List<AuditLogDto>>.Ok(logs));
    }
}

