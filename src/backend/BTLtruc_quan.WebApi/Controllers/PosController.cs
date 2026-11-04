using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BTLtruc_quan.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PosController : ControllerBase
{
    private readonly IPosService _posService;

    public PosController(IPosService posService)
    {
        _posService = posService;
    }

    /// <summary>
    /// Check-in nhận sân tại quầy POS theo mã đơn đặt hoặc SĐT (QT04)
    /// </summary>
    [HttpPost("checkin")]
    public async Task<ActionResult<ApiResponse<PosCheckInResponseDto>>> CheckIn([FromBody] PosCheckInRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _posService.CheckInAsync(request, cancellationToken);
            return Ok(ApiResponse<PosCheckInResponseDto>.Ok(result, "Check-in nhận sân thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<PosCheckInResponseDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<PosCheckInResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Bán thêm dịch vụ / nước uống / thuê vợt vào đơn đang thi đấu (trừ tồn kho)
    /// </summary>
    [HttpPost("add-service")]
    public async Task<ActionResult<ApiResponse<BookingServiceDetailDto>>> AddService([FromBody] PosAddServiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _posService.AddServiceAsync(request, cancellationToken);
            return Ok(ApiResponse<BookingServiceDetailDto>.Ok(result, "Đã thêm dịch vụ thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BookingServiceDetailDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BookingServiceDetailDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Check-out trả sân, thanh toán số dư còn lại (Pha 2), xuất hóa đơn POS và hoàn tất (QT07)
    /// </summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<ApiResponse<PosCheckoutResponseDto>>> Checkout([FromBody] PosCheckoutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _posService.CheckoutAsync(request, cancellationToken);
            return Ok(ApiResponse<PosCheckoutResponseDto>.Ok(result, "Thanh toán hóa đơn POS thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<PosCheckoutResponseDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PosCheckoutResponseDto>.Fail($"Lỗi khi thanh toán: {ex.Message}"));
        }
    }

    /// <summary>
    /// Lấy chi tiết đơn đặt phục vụ hiển thị màn hình POS thu ngân
    /// </summary>
    [HttpGet("booking/{id}")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetBookingDetails(int id, CancellationToken cancellationToken)
    {
        var result = await _posService.GetPosBookingDetailsAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse<BookingDto>.Fail($"Không tìm thấy đơn đặt #{id}."));
        }
        return Ok(ApiResponse<BookingDto>.Ok(result));
    }
}

