using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BTLtruc_quan.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchManagementController : ControllerBase
{
    private readonly IBranchManagementService _branchService;

    public BranchManagementController(IBranchManagementService branchService)
    {
        _branchService = branchService;
    }

    // 1. Quản lý Sân
    [HttpGet("courts")]
    public async Task<ActionResult<ApiResponse<List<CourtDto>>>> GetCourts([FromQuery] int branchId, CancellationToken cancellationToken)
    {
        var courts = await _branchService.GetCourtsByBranchAsync(branchId, cancellationToken);
        return Ok(ApiResponse<List<CourtDto>>.Ok(courts));
    }

    [HttpPost("courts")]
    public async Task<ActionResult<ApiResponse<CourtDto>>> CreateCourt([FromBody] CreateCourtRequest request, [FromQuery] string? performerName, CancellationToken cancellationToken)
    {
        string performer = !string.IsNullOrWhiteSpace(performerName) ? performerName : "Quản lý cơ sở";
        var court = await _branchService.CreateCourtAsync(request, performer, cancellationToken);
        return Ok(ApiResponse<CourtDto>.Ok(court, "Tạo sân thi đấu mới thành công."));
    }

    [HttpPut("courts/{id}")]
    public async Task<ActionResult<ApiResponse<CourtDto>>> UpdateCourt(int id, [FromBody] UpdateCourtRequest request, [FromQuery] string? performerName, CancellationToken cancellationToken)
    {
        string performer = !string.IsNullOrWhiteSpace(performerName) ? performerName : "Quản lý cơ sở";
        var court = await _branchService.UpdateCourtAsync(id, request, performer, cancellationToken);
        return Ok(ApiResponse<CourtDto>.Ok(court, "Cập nhật sân thi đấu thành công."));
    }

    [HttpPut("courts/{id}/maintenance")]
    public async Task<ActionResult<ApiResponse<bool>>> SetMaintenance(int id, [FromQuery] bool isMaintenance, [FromQuery] string? performerName, CancellationToken cancellationToken)
    {
        try
        {
            string performer = !string.IsNullOrWhiteSpace(performerName) ? performerName : "Quản lý cơ sở";
            bool result = await _branchService.SetCourtMaintenanceStatusAsync(id, isMaintenance, performer, cancellationToken);
            return Ok(ApiResponse<bool>.Ok(result, isMaintenance ? "Đã chuyển sân sang trạng thái bảo trì." : "Đã mở lại sân hoạt động."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail($"Lỗi khi đổi trạng thái bảo trì: {ex.Message}"));
        }
    }

    [HttpDelete("courts/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCourt(int id, [FromQuery] string? performerName, CancellationToken cancellationToken)
    {
        try
        {
            string performer = !string.IsNullOrWhiteSpace(performerName) ? performerName : "Quản lý cơ sở";
            bool result = await _branchService.SoftDeleteCourtAsync(id, performer, cancellationToken);
            return Ok(ApiResponse<bool>.Ok(result, "Đã xóa mềm sân thi đấu thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail($"Lỗi khi xóa sân: {ex.Message}"));
        }
    }

    // 2. Bảng giá ca sân
    [HttpGet("prices")]
    public async Task<ActionResult<ApiResponse<List<CourtPriceDto>>>> GetPrices([FromQuery] int branchId, CancellationToken cancellationToken)
    {
        var prices = await _branchService.GetPriceRulesAsync(branchId, cancellationToken);
        return Ok(ApiResponse<List<CourtPriceDto>>.Ok(prices));
    }

    [HttpPost("prices")]
    public async Task<ActionResult<ApiResponse<CourtPriceDto>>> SetPrice([FromBody] SetCourtPriceRequest request, [FromQuery] string? performerName, CancellationToken cancellationToken)
    {
        string performer = !string.IsNullOrWhiteSpace(performerName) ? performerName : "Quản lý cơ sở";
        var price = await _branchService.SetCourtPriceAsync(request, performer, cancellationToken);
        return Ok(ApiResponse<CourtPriceDto>.Ok(price, "Thiết lập bảng giá ca sân thành công."));
    }

    // 3. Dịch vụ & Kho
    [HttpGet("services")]
    public async Task<ActionResult<ApiResponse<List<ServiceItemDto>>>> GetServices([FromQuery] int branchId, CancellationToken cancellationToken)
    {
        var services = await _branchService.GetServiceItemsAsync(branchId, cancellationToken);
        return Ok(ApiResponse<List<ServiceItemDto>>.Ok(services));
    }

    [HttpPost("services")]
    public async Task<ActionResult<ApiResponse<ServiceItemDto>>> CreateService([FromBody] CreateServiceItemRequest request, [FromQuery] string? performerName, CancellationToken cancellationToken)
    {
        string performer = !string.IsNullOrWhiteSpace(performerName) ? performerName : "Quản lý cơ sở";
        var service = await _branchService.CreateServiceItemAsync(request, performer, cancellationToken);
        return Ok(ApiResponse<ServiceItemDto>.Ok(service, "Tạo mặt hàng dịch vụ thành công."));
    }

    [HttpPut("services/{id}/stock")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStock(int id, [FromQuery] int newStock, [FromQuery] string? performerName, CancellationToken cancellationToken)
    {
        try
        {
            string performer = !string.IsNullOrWhiteSpace(performerName) ? performerName : "Quản lý cơ sở";
            bool result = await _branchService.UpdateStockQuantityAsync(id, newStock, performer, cancellationToken);
            return Ok(ApiResponse<bool>.Ok(result, "Cập nhật tồn kho thành công."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail($"Lỗi khi cập nhật kho: {ex.Message}"));
        }
    }
}

