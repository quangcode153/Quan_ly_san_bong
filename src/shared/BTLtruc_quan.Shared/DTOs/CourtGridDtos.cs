using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.Shared.DTOs;

/// <summary>
/// DTO phục vụ trực tiếp Lưới Ma Trận Sân & Ca trực quan trên WinForms
/// </summary>
public class CourtGridResponseDto
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public DateTime SelectedDate { get; set; }
    public List<TimeSlotDto> TimeSlots { get; set; } = new();
    public List<CourtGridRowDto> Rows { get; set; } = new();
}

public class CourtGridRowDto
{
    public int CourtId { get; set; }
    public string CourtCode { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public CourtType CourtType { get; set; }
    public string CourtTypeName => CourtType.ToString();
    public List<CourtTimeSlotCellDto> Cells { get; set; } = new();
}

public class CourtTimeSlotCellDto
{
    public int CourtId { get; set; }
    public int TimeSlotId { get; set; }
    public string TimeSlotDisplayName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public decimal Price { get; set; }
    
    /// <summary>
    /// Trạng thái ô ca:
    /// Available = Trống (Xanh lá)
    /// PendingPayment = Chờ cọc (Vàng)
    /// Confirmed = Đã cọc (Cam)
    /// InUse = Đang thi đấu (Đỏ)
    /// Maintenance = Bảo trì (Xám)
    /// </summary>
    public string VisualStatus { get; set; } = "Available";
    
    public int? BookingId { get; set; }
    public string? BookingCode { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
}
