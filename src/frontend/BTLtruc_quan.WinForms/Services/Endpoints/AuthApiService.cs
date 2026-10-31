using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Services.Network;

namespace BTLtruc_quan.WinForms.Services.Endpoints;

/// <summary>
/// Service tương tác với các Endpoint xác thực (/api/auth)
/// </summary>
public class AuthApiService : BaseApiClient
{
    /// <summary>
    /// Đăng nhập tài khoản người dùng, tự động lưu trữ Token vào UserSession
    /// </summary>
    public static async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<LoginResponseDto>("auth/login", request, cancellationToken);
        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            UserSession.SetSession(response.Token, response.UserId, response.Username, response.FullName, response.RoleName);
        }
        return response!;
    }

    /// <summary>
    /// Lấy thông tin hồ sơ tài khoản hiện hành
    /// </summary>
    public static async Task<UserDto> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<UserDto>("auth/profile", cancellationToken);
    }
}

