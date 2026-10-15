using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.Shared.DTOs;

public class CourtDto
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CourtType CourtType { get; set; }
    public string CourtTypeName => CourtType.ToString();
    public string? SurfaceType { get; set; } // vd: Cỏ nhân tạo, Sàn gỗ, Thảm cao su
    public CourtStatus Status { get; set; } = CourtStatus.Active;
    public string StatusName => Status.ToString();
    public string? Description { get; set; }
    public decimal DefaultPricePerHour { get; set; }
}

public class CreateCourtRequest
{
    public int BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CourtType CourtType { get; set; } = CourtType.Football;
    public string? SurfaceType { get; set; }
    public string? Description { get; set; }
    public decimal DefaultPricePerHour { get; set; }
}

public class UpdateCourtRequest
{
    public string Name { get; set; } = string.Empty;
    public CourtType CourtType { get; set; }
    public string? SurfaceType { get; set; }
    public CourtStatus Status { get; set; }
    public string? Description { get; set; }
    public decimal DefaultPricePerHour { get; set; }
}

public class TimeSlotDto
{
    public int Id { get; set; }
    public int SlotIndex { get; set; } // 1, 2, 3...
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string DisplayName => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    public bool IsPeakHour { get; set; } // Thường là 17:00 - 22:00
}
