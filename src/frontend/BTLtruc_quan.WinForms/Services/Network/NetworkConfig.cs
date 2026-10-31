using BTLtruc_quan.Shared.Constants;

namespace BTLtruc_quan.WinForms.Services.Network;

/// <summary>
/// Cấu hình kết nối mạng tập trung cho WinForms Client
/// Trích xuất trực tiếp từ hằng số AppConstants trong Shared
/// </summary>
public static class NetworkConfig
{
    /// <summary>
    /// Địa chỉ gốc của Web API (Mặc định: http://localhost:5000/api)
    /// </summary>
    public static string BaseApiUrl { get; set; } = AppConstants.ApiBaseUrl.TrimEnd('/') + "/api";

    /// <summary>
    /// Địa chỉ SignalR Hub thời gian thực (Mặc định: http://localhost:5000/hubs/app)
    /// </summary>
    public static string RealtimeHubUrl { get; set; } = AppConstants.RealtimeHubUrl;

    /// <summary>
    /// Thời gian chờ yêu cầu HTTP tối đa (mặc định 15 giây)
    /// </summary>
    public static TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(15);
}

