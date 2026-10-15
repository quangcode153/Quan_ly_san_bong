namespace BTLtruc_quan.Shared.Enums;

/// <summary>
/// Trạng thái thanh toán tài chính của đơn đặt sân
/// </summary>
public enum PaymentStatus
{
    Unpaid = 1,       // Chưa thanh toán
    DepositPaid = 2,  // Đã thanh toán tiền cọc 30%
    FullyPaid = 3,    // Đã thanh toán toàn bộ 100% (tiền sân + dịch vụ phát sinh)
    Refunded = 4      // Đã hoàn lại tiền cọc (theo chính sách hủy trước 24h)
}
