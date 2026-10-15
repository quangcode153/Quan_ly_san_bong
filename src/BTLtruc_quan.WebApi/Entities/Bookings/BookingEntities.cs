using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Branches;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.Interactions;
using BTLtruc_quan.WebApi.Entities.Payments;
using BTLtruc_quan.WebApi.Entities.Users;

namespace BTLtruc_quan.WebApi.Entities.Bookings;

public class Booking : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public long CustomerId { get; set; }
    public long BranchId { get; set; }
    public BookingSource Source { get; set; } = BookingSource.Online;
    public BookingStatus Status { get; set; } = BookingStatus.Holding;
    public BookingPaymentStatus PaymentStatus { get; set; } = BookingPaymentStatus.Unpaid;
    public DateOnly PlayDate { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public decimal SlotAmount { get; set; }
    public decimal ServiceAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public int DepositPercentSnapshot { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public string? CancelPolicySnapshot { get; set; } // JSON
    public DateTime HoldExpiresAt { get; set; }

    public long? RecurringId { get; set; }
    public long? VoucherId { get; set; }
    public string QrSecret { get; set; } = string.Empty;

    public DateTime? CheckedInAt { get; set; }
    public CheckinMethod? CheckinMethod { get; set; }
    public string? CheckedInByName { get; set; }
    public DateTime? CompletedAt { get; set; }

    public CancelType? CancelType { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public long? CancelledByUserId { get; set; }
    public string? CancelledByName { get; set; }

    public long CreatedByUserId { get; set; }
    public string? CreatedByName { get; set; }
    public string? Note { get; set; }

    // Kiểm soát tương tranh lạc quan (Optimistic Concurrency Control) theo CONG_NGHE.md
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public User Customer { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public RecurringBooking? RecurringBooking { get; set; }
    public Voucher? Voucher { get; set; }
    public User? CancelledByUser { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public ICollection<BookingSlot> Slots { get; set; } = new List<BookingSlot>();
    public ICollection<BookingHistory> History { get; set; } = new List<BookingHistory>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Invoice? Invoice { get; set; }
    public Review? Review { get; set; }
}

public class BookingSlot : BaseEntity
{
    public long BookingId { get; set; }
    public long CourtId { get; set; }
    public long SlotId { get; set; }
    public DateOnly SlotDate { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public SlotType SlotTypeSnapshot { get; set; }
    public DayType DayTypeSnapshot { get; set; }
    public decimal PriceSnapshot { get; set; }
    public BookingSlotStatus Status { get; set; } = BookingSlotStatus.Active;

    public Booking Booking { get; set; } = null!;
    public Court Court { get; set; } = null!;
    public TimeSlot Slot { get; set; } = null!;
    public SlotLock? SlotLock { get; set; }
}

public class SlotLock
{
    public long Id { get; set; }
    public long CourtId { get; set; }
    public DateOnly SlotDate { get; set; }
    public long SlotId { get; set; }
    public SlotLockSourceType SourceType { get; set; }
    public long? BookingSlotId { get; set; }
    public long? BlockId { get; set; }
    public DateTime LockedAt { get; set; } = DateTime.UtcNow;

    // Optimistic Concurrency Control
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Court Court { get; set; } = null!;
    public TimeSlot Slot { get; set; } = null!;
    public BookingSlot? BookingSlot { get; set; }
    public SlotBlock? Block { get; set; }
}

public class BookingHistory
{
    public long Id { get; set; }
    public long BookingId { get; set; }
    public BookingAction Action { get; set; }
    public string? OldValue { get; set; } // JSON
    public string? NewValue { get; set; } // JSON
    public decimal PriceDiff { get; set; }
    public long? PerformedByUserId { get; set; }
    public string? PerformedByName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Booking Booking { get; set; } = null!;
    public User? PerformedByUser { get; set; }
}

public class RecurringBooking : BaseEntity
{
    public long CustomerId { get; set; }
    public long BranchId { get; set; }
    public long CourtId { get; set; }
    public byte Weekday { get; set; } // 1..7
    public long SlotId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int WeeksAhead { get; set; } = 4;
    public DateOnly GeneratedUntil { get; set; }
    public int DiscountPercent { get; set; }
    public RecurringStatus Status { get; set; } = RecurringStatus.Active;

    public User Customer { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Court Court { get; set; } = null!;
    public TimeSlot Slot { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<RecurringSkip> Skips { get; set; } = new List<RecurringSkip>();
}

public class RecurringSkip : BaseEntity
{
    public long RecurringId { get; set; }
    public DateOnly SkipDate { get; set; }
    public RecurringSkipReason Reason { get; set; }

    public RecurringBooking RecurringBooking { get; set; } = null!;
}
