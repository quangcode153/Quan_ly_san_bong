using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Services.Network;

namespace BTLtruc_quan.WinForms.Services.Endpoints;

/// <summary>
/// Service tương tác với Backend API Quản trị chi nhánh (/api/branchmanagement)
/// </summary>
public class BranchAdminApiService : BaseApiClient
{
    public static async Task<List<CourtDto>> GetCourtsAsync(int branchId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<CourtDto>>($"branchmanagement/courts?branchId={branchId}", cancellationToken);
    }

    public static async Task<CourtDto> CreateCourtAsync(CreateCourtRequest request, string performerName, CancellationToken cancellationToken = default)
    {
        return await PostAsync<CourtDto>($"branchmanagement/courts?performerName={Uri.EscapeDataString(performerName)}", request, cancellationToken);
    }

    public static async Task<bool> SetMaintenanceAsync(int courtId, bool isMaintenance, string performerName, CancellationToken cancellationToken = default)
    {
        return await PutAsync<bool>($"branchmanagement/courts/{courtId}/maintenance?isMaintenance={isMaintenance}&performerName={Uri.EscapeDataString(performerName)}", null, cancellationToken);
    }

    public static async Task<List<CourtPriceDto>> GetPriceRulesAsync(int branchId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<CourtPriceDto>>($"branchmanagement/prices?branchId={branchId}", cancellationToken);
    }

    public static async Task<CourtPriceDto> SetPriceRuleAsync(SetCourtPriceRequest request, string performerName, CancellationToken cancellationToken = default)
    {
        return await PostAsync<CourtPriceDto>($"branchmanagement/prices?performerName={Uri.EscapeDataString(performerName)}", request, cancellationToken);
    }

    public static async Task<List<ServiceItemDto>> GetServicesAsync(int branchId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<ServiceItemDto>>($"branchmanagement/services?branchId={branchId}", cancellationToken);
    }

    public static async Task<bool> UpdateStockAsync(int serviceItemId, int newStock, string performerName, CancellationToken cancellationToken = default)
    {
        return await PutAsync<bool>($"branchmanagement/services/{serviceItemId}/stock?newStock={newStock}&performerName={Uri.EscapeDataString(performerName)}", null, cancellationToken);
    }
}

