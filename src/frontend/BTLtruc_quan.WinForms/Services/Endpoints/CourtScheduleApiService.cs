using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Services.Network;

namespace BTLtruc_quan.WinForms.Services.Endpoints;

/// <summary>
/// Service tương tác với các Endpoint Lịch sân và Ma trận sân (/api/courts)
/// </summary>
public class CourtScheduleApiService : BaseApiClient
{
    /// <summary>
    /// Lấy ma trận sân và khung giờ trực quan theo Chi nhánh và Ngày chọn
    /// </summary>
    public static async Task<CourtGridResponseDto> GetCourtMatrixAsync(int branchId, DateTime date, CancellationToken cancellationToken = default)
    {
        string endpoint = $"courts/matrix?branchId={branchId}&date={date:yyyy-MM-dd}";
        return await GetAsync<CourtGridResponseDto>(endpoint, cancellationToken);
    }
}

