using BTLtruc_quan.Shared.DTOs;

namespace BTLtruc_quan.WebApi.Services;

/// <summary>
/// Giao diện xử lý nghiệp vụ Quầy thu ngân POS (Check-in, Bán dịch vụ, Check-out & In hóa đơn)
/// Căn cứ: doc/specifications/BUSINESS_RULES_AND_STATE_MACHINE.md (QT04, QT07, QT18)
/// </summary>
public interface IPosService
{
    /// <summary>
    /// Check-in nhận sân cho khách hàng (theo mã BookingCode hoặc SĐT)
    /// </summary>
    Task<PosCheckInResponseDto> CheckInAsync(PosCheckInRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Thêm dịch vụ / nước uống / thuê vợt vào đơn đặt đang thi đấu (trừ kho)
    /// </summary>
    Task<BookingServiceDetailDto> AddServiceAsync(PosAddServiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check-out ca sân, tính tiền hạch toán Pha 2, xuất hóa đơn POS và hoàn tất đơn
    /// </summary>
    Task<PosCheckoutResponseDto> CheckoutAsync(PosCheckoutRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy chi tiết thông tin đơn đặt phục vụ màn hình POS thu ngân
    /// </summary>
    Task<BookingDto?> GetPosBookingDetailsAsync(int bookingId, CancellationToken cancellationToken = default);
}

