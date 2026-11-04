namespace BTLtruc_quan.Shared.DTOs;

/// <summary>
/// DTO phục vụ Nhật ký kiểm toán hoạt động (Audit Log - QT18 & Đặc tả mục 1.4)
/// Lưu vết chi tiết người thực hiện, trước/sau của các hành động nhạy cảm.
/// </summary>
public class AuditLogDto
{
    public long Id { get; set; }
    public int? ActorUserId { get; set; }
    public string PerformerName { get; set; } = string.Empty; // Tên nhân viên trực quầy (Mục 1.4)
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // CHECKIN, POS_CHECKOUT, ADD_SERVICE, CREATE_COURT, UPDATE_PRICE, CANCEL
    public string EntityName { get; set; } = string.Empty; // Booking, Court, PriceRule, ServiceItem
    public string EntityId { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAuditLogRequest
{
    public int? ActorUserId { get; set; }
    public string PerformerName { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Notes { get; set; }
}

public class AuditLogFilterRequest
{
    public int? BranchId { get; set; }
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public string? SearchKeyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

