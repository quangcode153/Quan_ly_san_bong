using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.Services;
using BTLtruc_quan.WebApi.Entities.Users;

namespace BTLtruc_quan.WebApi.Entities.Branches;

public class Branch : BaseEntity, ISoftDelete
{
    public long OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? District { get; set; }
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public BranchStatus Status { get; set; } = BranchStatus.Pending;
    public string? RejectReason { get; set; }
    public string? LockReason { get; set; }
    public long? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public decimal AvgRating { get; set; } = 0.0m;
    public DateTime? DeletedAt { get; set; }

    public User OwnerUser { get; set; } = null!;
    public User? ApprovedByUser { get; set; }

    public ICollection<BranchImage> Images { get; set; } = new List<BranchImage>();
    public ICollection<BranchAmenity> BranchAmenities { get; set; } = new List<BranchAmenity>();
    public BranchPolicy? Policy { get; set; }
    public ICollection<CancelPolicyTier> CancelPolicyTiers { get; set; } = new List<CancelPolicyTier>();
    public ICollection<Court> Courts { get; set; } = new List<Court>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    public ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<BranchStaffName> StaffNames { get; set; } = new List<BranchStaffName>();
}

public class BranchImage : BaseEntity
{
    public long BranchId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsCover { get; set; }

    public Branch Branch { get; set; } = null!;
}

public class Amenity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }

    public ICollection<BranchAmenity> BranchAmenities { get; set; } = new List<BranchAmenity>();
}

public class BranchAmenity
{
    public long BranchId { get; set; }
    public long AmenityId { get; set; }

    public Branch Branch { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}

public class BranchPolicy : BaseEntity
{
    public long BranchId { get; set; }
    public int DepositPercent { get; set; } = 30;
    public bool AllowChangeBooking { get; set; } = true;
    public int ChangeBeforeHours { get; set; } = 12;
    public int MaxHoldExtend { get; set; } = 0;

    public Branch Branch { get; set; } = null!;
}

public class CancelPolicyTier : BaseEntity
{
    public long? BranchId { get; set; }
    public int HoursBefore { get; set; }
    public int RefundPercent { get; set; }
    public int SortOrder { get; set; }

    public Branch? Branch { get; set; }
}
