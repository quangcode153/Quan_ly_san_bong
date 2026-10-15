using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Branches;

namespace BTLtruc_quan.WebApi.Entities.Payments;

public class Payment : BaseEntity
{
    public long BookingId { get; set; }
    public PaymentType Type { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentGateway? Gateway { get; set; }
    public decimal Amount { get; set; }
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;
    public string? GatewayTxnId { get; set; }
    public string? GatewayPayload { get; set; } // JSON
    public long? RelatedPaymentId { get; set; }
    public string? ReceivedByName { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Note { get; set; }

    public Booking Booking { get; set; } = null!;
    public Payment? RelatedPayment { get; set; }
}

public class Invoice : BaseEntity
{
    public long BookingId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public string? IssuedByName { get; set; }
    public string? PdfPath { get; set; }

    public Booking Booking { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}

public class InvoiceItem : BaseEntity
{
    public long InvoiceId { get; set; }
    public InvoiceItemType ItemType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }

    public Invoice Invoice { get; set; } = null!;
}

public class DailyClosing : BaseEntity
{
    public long BranchId { get; set; }
    public DateOnly ClosingDate { get; set; }
    public decimal CashTotal { get; set; }
    public decimal TransferTotal { get; set; }
    public decimal CardTotal { get; set; }
    public decimal ActualCash { get; set; }
    public decimal Difference { get; set; }
    public string? ClosedByName { get; set; }
    public string? Note { get; set; }

    public Branch Branch { get; set; } = null!;
}

public class Voucher : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public long? BranchId { get; set; }
    public VoucherDiscountType DiscountType { get; set; } = VoucherDiscountType.Percent;
    public decimal Value { get; set; }
    public decimal? MaxDiscount { get; set; }
    public decimal? MinOrder { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; } = 0;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Branch? Branch { get; set; }
}
