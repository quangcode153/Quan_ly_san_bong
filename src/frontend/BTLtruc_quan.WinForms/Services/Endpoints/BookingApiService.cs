using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Services.Network;

namespace BTLtruc_quan.WinForms.Services.Endpoints;

/// <summary>
/// Service tương tác với các Endpoint Đặt sân, Check-in, Hủy ca (/api/bookings)
/// </summary>
public class BookingApiService : BaseApiClient
{
    /// <summary>
    /// Tạo đơn đặt sân mới hoặc giữ chỗ tại quầy
    /// </summary>
    public static async Task<BookingDto> CreateBookingAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<BookingDto>("bookings", request, cancellationToken);
    }

    /// <summary>
    /// Check-in nhận sân cho khách hàng theo mã đơn đặt
    /// </summary>
    public static async Task<BookingDto> CheckInAsync(string bookingCode, string? staffNotes = null, CancellationToken cancellationToken = default)
    {
        var request = new CheckInRequest
        {
            BookingCode = bookingCode,
            StaffNotes = staffNotes
        };
        return await PostAsync<BookingDto>("bookings/checkin", request, cancellationToken);
    }

    /// <summary>
    /// Check-in nhận sân theo Id đơn đặt
    /// </summary>
    public static async Task<BookingDto> CheckInAsync(int bookingId, string? staffNotes = null, CancellationToken cancellationToken = default)
    {
        var request = new CheckInRequest
        {
            BookingCode = bookingId.ToString(),
            StaffNotes = staffNotes
        };
        return await PostAsync<BookingDto>($"bookings/{bookingId}/checkin", request, cancellationToken);
    }

    /// <summary>
    /// Hủy đơn đặt sân theo ID và lý do
    /// </summary>
    public static async Task<bool> CancelBookingAsync(int bookingId, string reason, CancellationToken cancellationToken = default)
    {
        var request = new CancelBookingRequest
        {
            BookingId = bookingId,
            Reason = reason
        };
        return await PostAsync<bool>($"bookings/{bookingId}/cancel", request, cancellationToken);
    }
}

