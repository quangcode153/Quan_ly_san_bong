using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.Shared.DTOs;

public class CourtPriceDto
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public CourtType CourtType { get; set; }
    public string CourtTypeName => CourtType.ToString();
    public int TimeSlotId { get; set; }
    public string TimeSlotName { get; set; } = string.Empty;
    public DayOfWeek? SpecificDayOfWeek { get; set; } // null = áp dụng mọi ngày, hoặc Thứ 7 / Chủ Nhật
    public decimal PricePerHour { get; set; }
    public bool IsPeakHour { get; set; }
}

public class SetCourtPriceRequest
{
    public int BranchId { get; set; }
    public CourtType CourtType { get; set; }
    public int TimeSlotId { get; set; }
    public DayOfWeek? SpecificDayOfWeek { get; set; }
    public decimal PricePerHour { get; set; }
}
