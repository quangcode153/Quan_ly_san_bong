using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.Shared.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal RemainingAmount => TotalAmount - DepositAmount;
    
    public BookingStatus BookingStatus { get; set; }
    public string BookingStatusName => BookingStatus.ToString();
    
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusName => PaymentStatus.ToString();
    
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; } // Hết hạn 15 phút nếu chưa cọc
    
    public List<BookingDetailDto> Details { get; set; } = new();
}

public class BookingDetailDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public CourtType CourtType { get; set; }
    public string CourtTypeName => CourtType.ToString();
    public DateTime BookingDate { get; set; }
    public int TimeSlotId { get; set; }
    public string TimeSlotDisplayName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CreateBookingRequest
{
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int CourtId { get; set; }
    public DateTime BookingDate { get; set; }
    public List<int> TimeSlotIds { get; set; } = new(); // Danh sách ca đặt liên tiếp
    public string? Notes { get; set; }
    public bool PayFullAtCounter { get; set; } = false; // Đặt tại quầy thanh toán 100%
}

public class BookingFilterRequest
{
    public int? BranchId { get; set; }
    public CourtType? CourtType { get; set; }
    public BookingStatus? BookingStatus { get; set; }
    public DateTime? BookingDate { get; set; }
    public string? SearchKeyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CheckInRequest
{
    public string BookingCode { get; set; } = string.Empty;
    public string? StaffNotes { get; set; }
}

public class CancelBookingRequest
{
    public int BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
