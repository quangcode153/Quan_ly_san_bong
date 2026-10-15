using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.Shared.DTOs;

public class ServiceItemDto
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Nước giải khát, Thuê vợt, Thuê bóng, v.v.
    public decimal Price { get; set; }
    public string Unit { get; set; } = "Chai"; // Chai, Lon, Quả, Cái, Giờ
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateServiceItemRequest
{
    public int BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "Nước giải khát";
    public decimal Price { get; set; }
    public string Unit { get; set; } = "Chai";
    public int StockQuantity { get; set; }
}

public class UpdateServiceItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
}

public class BookingServiceDetailDto
{
    public int ServiceItemId { get; set; }
    public string ServiceItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}

public class AddServiceToBookingRequest
{
    public int ServiceItemId { get; set; }
    public int Quantity { get; set; }
}

public class CheckoutPosRequest
{
    public int BookingId { get; set; }
    public List<AddServiceToBookingRequest> AdditionalServices { get; set; } = new();
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal CashGiven { get; set; } // Số tiền khách đưa tại quầy
    public string? Notes { get; set; }
}

public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    public decimal CourtFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal TotalAmount => CourtFee + ServiceFee;
    
    public decimal DepositPaid { get; set; }
    public decimal FinalPaid => TotalAmount - DepositPaid;
    
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName => PaymentMethod.ToString();
    public string CashierName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public List<BookingServiceDetailDto> ServiceDetails { get; set; } = new();
}
