namespace BTLtruc_quan.Shared.Enums;

/// <summary>
/// Trạng thái hoạt động của cơ sở vật chất sân
/// </summary>
public enum CourtStatus
{
    Active = 1,      // Sân đang hoạt động bình thường, sẵn sàng cho thuê
    Maintenance = 2, // Sân đang sửa chữa / bảo trì (khóa trên lưới)
    Inactive = 3     // Tạm ngưng hoạt động
}
