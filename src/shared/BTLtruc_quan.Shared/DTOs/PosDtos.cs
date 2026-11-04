using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.Shared.DTOs;

/// <summary>
/// DTOs phục vụ nghiệp vụ Thu ngân POS tại quầy (Check-in, Bán nước/vợt, Check-out & In hóa đơn)
/// Căn cứ: doc/specifications/BUSINESS_RULES_AND_STATE_MACHINE.md (QT04, QT07, QT18)
/// </summary>
public class PosCheckInRequest
{
    public string BookingCode { get; set; } = string.Empty;
    public string PerformerName { get; set; } = string.Empty; // Tên nhân viên trực quầy (Mục 1.4)
    public string? StaffNotes { get; set; }
}

public class PosCheckInResponseDto
{
    public int BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string TimeSlotDisplayName { get; set; } = string.Empty;
    
    // Hạch toán 2 pha: Tiền cọc Pha 1 và Tiền còn nợ
    public decimal TotalCourtFee { get; set; }
    public decimal DepositPaid { get; set; }
    public decimal RemainingCourtFee => TotalCourtFee - DepositPaid;
    
    public BookingStatus Status { get; set; }
    public DateTime CheckInTime { get; set; }
}

public class PosAddServiceRequest
{
    public int BookingId { get; set; }
    public int ServiceItemId { get; set; }
    public int Quantity { get; set; }
    public string PerformerName { get; set; } = string.Empty;
}

public class PosCheckoutRequest
{
    public int BookingId { get; set; }
    public string PerformerName { get; set; } = string.Empty; // Tên thu ngân thực hiện
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal CashGiven { get; set; } // Tiền khách đưa tại quầy
    public decimal DiscountAmount { get; set; } = 0; // Giảm giá / Khuyến mãi
    public string? Notes { get; set; }
    public List<AddServiceToBookingRequest> AdditionalServices { get; set; } = new();
}

public class PosCheckoutResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public InvoiceDto Invoice { get; set; } = null!;
    public decimal CashReturn { get; set; } // Tiền thối lại cho khách
}

