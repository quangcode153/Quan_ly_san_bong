using BTLtruc_quan.Shared.DTOs;

namespace BTLtruc_quan.WebApi.Services;

/// <summary>
/// Giao diện ghi và truy vấn Nhật ký kiểm toán (Audit Log - QT18 & Đặc tả mục 1.4)
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Ghi nhận 1 sự kiện kiểm toán bất đồng bộ
    /// </summary>
    Task LogAsync(CreateAuditLogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Truy vấn danh sách nhật ký kiểm toán có phân trang và bộ lọc
    /// </summary>
    Task<List<AuditLogDto>> GetLogsAsync(AuditLogFilterRequest filter, CancellationToken cancellationToken = default);
}

