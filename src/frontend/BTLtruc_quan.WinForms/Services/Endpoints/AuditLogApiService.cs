using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Services.Network;

namespace BTLtruc_quan.WinForms.Services.Endpoints;

/// <summary>
/// Service tương tác với Backend API Tra cứu Nhật ký kiểm toán (/api/audit-logs)
/// </summary>
public class AuditLogApiService : BaseApiClient
{
    public static async Task<List<AuditLogDto>> GetLogsAsync(AuditLogFilterRequest filter, CancellationToken cancellationToken = default)
    {
        string endpoint = $"audit-logs?branchId={filter.BranchId}&action={filter.Action}&pageIndex={filter.PageIndex}&pageSize={filter.PageSize}";
        return await GetAsync<List<AuditLogDto>>(endpoint, cancellationToken);
    }
}

