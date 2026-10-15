using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Branches;
using BTLtruc_quan.WebApi.Entities.Courts;

namespace BTLtruc_quan.WebApi.Entities.Services;

public class Service : BaseEntity, ISoftDelete
{
    public long BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int? StockQty { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public Branch Branch { get; set; } = null!;
}

public class BookingService : BaseEntity
{
    public long BookingId { get; set; }
    public long ServiceId { get; set; }
    public string NameSnapshot { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPriceSnapshot { get; set; }
    public decimal Amount { get; set; }
    public ServiceAddedWhen AddedWhen { get; set; } = ServiceAddedWhen.AtBooking;
    public string? AddedByName { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public Booking Booking { get; set; } = null!;
    public Service Service { get; set; } = null!;
}

public class Equipment : BaseEntity, ISoftDelete
{
    public long CourtId { get; set; }
    public string Name { get; set; } = string.Empty;
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Good;
    public string? Note { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Court Court { get; set; } = null!;
}

public class Incident : BaseEntity
{
    public long CourtId { get; set; }
    public long? EquipmentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Low;
    public string? ImageUrl { get; set; }
    public string? ReportedByName { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public bool SetMaintenance { get; set; }
    public string? ResolvedNote { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Court Court { get; set; } = null!;
    public Equipment? Equipment { get; set; }
}
