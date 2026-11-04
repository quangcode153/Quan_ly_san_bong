using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Services.Network;

namespace BTLtruc_quan.WinForms.Services.Endpoints;

/// <summary>
/// Service tương tác với Backend API POS Thu ngân tại quầy (/api/pos)
/// </summary>
public class PosApiService : BaseApiClient
{
    /// <summary>
    /// Check-in nhận sân cho khách hàng (QT04)
    /// </summary>
    public static async Task<PosCheckInResponseDto> CheckInAsync(PosCheckInRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<PosCheckInResponseDto>("pos/checkin", request, cancellationToken);
    }

    /// <summary>
    /// Bán thêm dịch vụ / nước uống / thuê dụng cụ vào đơn (trừ kho)
    /// </summary>
    public static async Task<BookingServiceDetailDto> AddServiceAsync(PosAddServiceRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<BookingServiceDetailDto>("pos/add-service", request, cancellationToken);
    }

    /// <summary>
    /// Check-out thanh toán nốt tiền dư, tạo hóa đơn POS và hoàn tất đơn (QT07)
    /// </summary>
    public static async Task<PosCheckoutResponseDto> CheckoutAsync(PosCheckoutRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<PosCheckoutResponseDto>("pos/checkout", request, cancellationToken);
    }

    /// <summary>
    /// Lấy chi tiết đơn đặt phục vụ hiển thị màn hình POS
    /// </summary>
    public static async Task<BookingDto> GetBookingDetailsAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<BookingDto>($"pos/booking/{bookingId}", cancellationToken);
    }
}

