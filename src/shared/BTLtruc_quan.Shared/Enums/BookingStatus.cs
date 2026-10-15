namespace BTLtruc_quan.Shared.Enums;

/// <summary>
/// Vòng đời trạng thái của đơn đặt sân (Theo đặc tả kỹ thuật)
/// </summary>
public enum BookingStatus
{
    PendingPayment = 1, // Đang giữ chỗ tạm thời (khóa 15 phút chờ chuyển khoản cọc)
    Confirmed = 2,      // Đã cọc 30% / Đã xác nhận lịch chắc chắn
    CheckedIn = 3,      // Khách đã đến sân nhận ca / Đang thi đấu
    Completed = 4,      // Đã trả sân, thanh toán 100% hoàn tất
    Cancelled = 5,      // Đã hủy đơn (theo chính sách hủy)
    Expired = 6,        // Hết hạn giữ chỗ 15 phút (tự động giải phóng ca)
    NoShow = 7          // Khách bùng lịch / không đến (tịch thu cọc)
}
