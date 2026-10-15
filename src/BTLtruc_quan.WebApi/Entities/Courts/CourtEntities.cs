using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Branches;

namespace BTLtruc_quan.WebApi.Entities.Courts;

public class CourtType : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public ICollection<Court> Courts { get; set; } = new List<Court>();
}

public class Court : BaseEntity, ISoftDelete
{
    public long BranchId { get; set; }
    public long CourtTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Capacity { get; set; }
    public CourtStatus Status { get; set; } = CourtStatus.Active;
    public CourtLiveStatus LiveStatus { get; set; } = CourtLiveStatus.Available;
    public DateTime? LiveStatusManualUntil { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Branch Branch { get; set; } = null!;
    public CourtType CourtType { get; set; } = null!;
    public ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();
    public ICollection<SlotBlock> SlotBlocks { get; set; } = new List<SlotBlock>();
}

public class TimeSlot : BaseEntity
{
    public long BranchId { get; set; }
    public int SlotIndex { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public SlotType SlotType { get; set; } = SlotType.OffPeak;
    public bool IsActive { get; set; } = true;

    public Branch Branch { get; set; } = null!;
}

public class PriceRule : BaseEntity
{
    public long BranchId { get; set; }
    public long? CourtTypeId { get; set; }
    public long? CourtId { get; set; }
    public DayType DayType { get; set; }
    public SlotType SlotType { get; set; }
    public decimal Price { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }

    public Branch Branch { get; set; } = null!;
    public CourtType? CourtType { get; set; }
    public Court? Court { get; set; }
}

public class SlotBlock : BaseEntity
{
    public long CourtId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long? StartSlotId { get; set; }
    public long? EndSlotId { get; set; }
    public BlockType BlockType { get; set; }
    public string? Reason { get; set; }
    public string? PerformedByName { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Court Court { get; set; } = null!;
    public TimeSlot? StartSlot { get; set; }
    public TimeSlot? EndSlot { get; set; }
    public ICollection<SlotLock> Locks { get; set; } = new List<SlotLock>();
}

public class Holiday : BaseEntity
{
    public long? BranchId { get; set; }
    public DateOnly HolidayDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SurchargePercent { get; set; }

    public Branch? Branch { get; set; }
}
