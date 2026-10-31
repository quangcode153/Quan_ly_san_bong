using System.Net;
using System.Net.Http.Headers;

namespace BTLtruc_quan.WinForms.Services.Network;

/// <summary>
/// Http Message Handler tự động chèn JWT Bearer Token vào mọi Request
/// và bắt mã lỗi 401 Unauthorized để kích hoạt sự kiện hết hạn phiên
/// </summary>
public class JwtAuthHeaderHandler : DelegatingHandler
{
    public JwtAuthHeaderHandler()
    {
        InnerHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        };
    }

    public JwtAuthHeaderHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Tự động đính kèm Token nếu phiên hiện hành có AccessToken
        if (!string.IsNullOrWhiteSpace(UserSession.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.AccessToken);
        }

        // 2. Gửi request tới server
        var response = await base.SendAsync(request, cancellationToken);

        // 3. Nếu server trả về 401 Unauthorized, thông báo phiên làm việc hết hạn
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            UserSession.TriggerSessionExpired();
        }

        return response;
    }
}

