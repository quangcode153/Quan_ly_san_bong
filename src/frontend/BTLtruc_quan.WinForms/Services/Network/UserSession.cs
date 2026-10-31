namespace BTLtruc_quan.WinForms.Services.Network;

/// <summary>
/// Quản lý phiên làm việc tập trung (User Session) cho WinForms Desktop
/// Hỗ trợ cờ DevMode để test độc lập và event thông báo token hết hạn an toàn luồng
/// </summary>
public static class UserSession
{
    public static string? AccessToken { get; set; }
    public static string? RefreshToken { get; set; }
    public static int? UserId { get; set; }
    public static string FullName { get; set; } = "Huỳnh Tấn Phát";
    public static string Username { get; set; } = "phat.ht";
    public static string Role { get; set; } = "Staff";
    public static int BranchId { get; set; } = 1;
    public static string BranchName { get; set; } = "SportChain - Chi nhánh Quận 7";
    public static bool IsDevMode { get; set; } = true;

    /// <summary>
    /// Trạng thái đã đăng nhập hợp lệ hoặc đang bật DevMode
    /// </summary>
    public static bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken) || IsDevMode;

    /// <summary>
    /// Sự kiện kích hoạt khi Token hết hạn (401 Unauthorized)
    /// </summary>
    public static event Action? OnSessionExpired;

    /// <summary>
    /// Bắn tín hiệu phiên hết hạn an toàn
    /// </summary>
    public static void TriggerSessionExpired()
    {
        OnSessionExpired?.Invoke();
    }

    /// <summary>
    /// Nạp sẵn dữ liệu ca trực mẫu (Lễ tân Huỳnh Tấn Phát) để kiểm thử độc lập
    /// </summary>
    public static void EnableDevMode()
    {
        IsDevMode = true;
        AccessToken = "dev_token_huynh_tan_phat_sportchain_q7";
        UserId = 1;
        FullName = "Huỳnh Tấn Phát";
        Username = "phat.ht";
        Role = "Staff";
        BranchId = 1;
        BranchName = "SportChain - Chi nhánh Quận 7";
    }

    /// <summary>
    /// Cập nhật phiên đăng nhập từ kết quả API
    /// </summary>
    public static void SetSession(string token, int userId, string username, string fullName, string role, int branchId = 1, string branchName = "SportChain - Chi nhánh Quận 7")
    {
        AccessToken = token;
        UserId = userId;
        Username = username;
        FullName = fullName;
        Role = role;
        BranchId = branchId;
        BranchName = branchName;
        IsDevMode = false;
    }

    /// <summary>
    /// Xóa phiên làm việc khi đăng xuất
    /// </summary>
    public static void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        UserId = null;
        FullName = string.Empty;
        Username = string.Empty;
        Role = string.Empty;
        IsDevMode = false;
    }
}

