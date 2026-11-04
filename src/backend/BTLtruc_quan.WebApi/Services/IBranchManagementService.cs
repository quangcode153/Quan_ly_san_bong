using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.Shared.Enums;

namespace BTLtruc_quan.WebApi.Services;

/// <summary>
/// Giao diện xử lý nghiệp vụ Quản trị cơ sở Chi nhánh (Quản lý sân, Bảng giá, Dịch vụ kho)
/// Căn cứ: doc/Đặc tả chức năng - Hệ thống quản lý chuỗi cho thuê sân thể thao.md (Mục 2 & Mục 3)
/// </summary>
public interface IBranchManagementService
{
    // 1. Quản lý Sân thi đấu
    Task<List<CourtDto>> GetCourtsByBranchAsync(int branchId, CancellationToken cancellationToken = default);
    Task<CourtDto> CreateCourtAsync(CreateCourtRequest request, string performerName, CancellationToken cancellationToken = default);
    Task<CourtDto> UpdateCourtAsync(int courtId, UpdateCourtRequest request, string performerName, CancellationToken cancellationToken = default);
    Task<bool> SetCourtMaintenanceStatusAsync(int courtId, bool isMaintenance, string performerName, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteCourtAsync(int courtId, string performerName, CancellationToken cancellationToken = default);

    // 2. Cấu hình Bảng giá ca sân (Phụ thu cao điểm / cuối tuần)
    Task<List<CourtPriceDto>> GetPriceRulesAsync(int branchId, CancellationToken cancellationToken = default);
    Task<CourtPriceDto> SetCourtPriceAsync(SetCourtPriceRequest request, string performerName, CancellationToken cancellationToken = default);

    // 3. Quản lý danh mục Dịch vụ & Tồn kho
    Task<List<ServiceItemDto>> GetServiceItemsAsync(int branchId, CancellationToken cancellationToken = default);
    Task<ServiceItemDto> CreateServiceItemAsync(CreateServiceItemRequest request, string performerName, CancellationToken cancellationToken = default);
    Task<ServiceItemDto> UpdateServiceItemAsync(int serviceItemId, UpdateServiceItemRequest request, string performerName, CancellationToken cancellationToken = default);
    Task<bool> UpdateStockQuantityAsync(int serviceItemId, int newStock, string performerName, CancellationToken cancellationToken = default);
}

