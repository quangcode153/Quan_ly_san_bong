using Microsoft.AspNetCore.SignalR.Client;
using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Services.Network;

namespace BTLtruc_quan.WinForms.Services.Realtime;

/// <summary>
/// Client kết nối SignalR Hub thời gian thực phục vụ đồng bộ Ma trận Lịch sân tức thời
/// </summary>
public class CourtRealtimeClient : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private bool _isDisposed = false;

    /// <summary>
    /// Sự kiện khi ô ca sân được cập nhật trạng thái từ máy chủ
    /// </summary>
    public event Action<CourtTimeSlotCellDto>? OnCellUpdated;

    /// <summary>
    /// Sự kiện thông báo trạng thái kết nối (IsConnected, Message)
    /// </summary>
    public event Action<bool, string>? OnConnectionStateChanged;

    /// <summary>
    /// Trạng thái kết nối hiện tại tới Hub
    /// </summary>
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    /// <summary>
    /// Khởi động kết nối SignalR Hub
    /// </summary>
    public async Task StartAsync()
    {
        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
            return;

        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NetworkConfig.RealtimeHubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(UserSession.AccessToken);
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            // 1. Lắng nghe event đẩy toàn bộ DTO ô ca sân
            _hubConnection.On<CourtTimeSlotCellDto>("CourtStatusChanged", cell =>
            {
                OnCellUpdated?.Invoke(cell);
            });

            // 2. Lắng nghe event đẩy các tham số rời rạc
            _hubConnection.On<int, int, string, string?, string?>("CourtCellUpdated", (courtId, timeSlotId, status, bookingCode, customerName) =>
            {
                var cell = new CourtTimeSlotCellDto
                {
                    CourtId = courtId,
                    TimeSlotId = timeSlotId,
                    VisualStatus = status,
                    BookingCode = bookingCode,
                    CustomerName = customerName
                };
                OnCellUpdated?.Invoke(cell);
            });

            // 3. Xử lý các trạng thái kết nối
            _hubConnection.Reconnecting += error =>
            {
                OnConnectionStateChanged?.Invoke(false, "Đang thử kết nối lại với máy chủ SignalR...");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                OnConnectionStateChanged?.Invoke(true, "Đã khôi phục kết nối thời gian thực thành công.");
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                OnConnectionStateChanged?.Invoke(false, "Kết nối thời gian thực đã đóng.");
                return Task.CompletedTask;
            };

            await _hubConnection.StartAsync();
            OnConnectionStateChanged?.Invoke(true, "Đã kết nối SignalR Hub thành công.");
        }
        catch (Exception ex)
        {
            OnConnectionStateChanged?.Invoke(false, $"Chưa thể kết nối SignalR Hub ({NetworkConfig.RealtimeHubUrl}): {ex.Message}");
        }
    }

    /// <summary>
    /// Ngắt kết nối SignalR Hub
    /// </summary>
    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                await _hubConnection.StopAsync();
            }
            catch { }
        }
    }

    /// <summary>
    /// Giải phóng tài nguyên kết nối
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }
    }
}

