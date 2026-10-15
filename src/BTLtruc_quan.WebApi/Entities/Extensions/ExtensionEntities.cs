using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Branches;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.Users;

namespace BTLtruc_quan.WebApi.Entities.Extensions;

public class Waitlist : BaseEntity
{
    public long CustomerId { get; set; }
    public long CourtId { get; set; }
    public DateOnly SlotDate { get; set; }
    public long SlotId { get; set; }
    public WaitlistStatus Status { get; set; } = WaitlistStatus.Waiting;
    public DateTime? NotifiedAt { get; set; }

    public User Customer { get; set; } = null!;
    public Court Court { get; set; } = null!;
    public TimeSlot Slot { get; set; } = null!;
}

public class Favorite : BaseEntity
{
    public long CustomerId { get; set; }
    public long BranchId { get; set; }
    public long? CourtId { get; set; }

    public User Customer { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Court? Court { get; set; }
}
