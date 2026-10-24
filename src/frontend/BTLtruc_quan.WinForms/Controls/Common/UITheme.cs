using System.Drawing;

namespace BTLtruc_quan.WinForms.Controls.Common;

/// <summary>
/// Bảng quy chuẩn mã màu và kiểu dáng giao diện thương hiệu SportChain VN
/// Áp dụng thống nhất cho toàn bộ hệ thống Custom Controls và WinForms.
/// </summary>
public static class UITheme
{
    // ==========================================
    // 1. BRAND COLORS (EMERALD GREEN PALETTE)
    // ==========================================
    public static readonly Color Primary = ColorTranslator.FromHtml("#10B981");
    public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#059669");
    public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#D1FAE5");

    // ==========================================
    // 2. BACKGROUNDS & SURFACES
    // ==========================================
    public static readonly Color DarkNavy = ColorTranslator.FromHtml("#0F172A");
    public static readonly Color NavySurface = ColorTranslator.FromHtml("#1E293B");
    public static readonly Color LightBackground = ColorTranslator.FromHtml("#F8FAFC");
    public static readonly Color CardBackground = ColorTranslator.FromHtml("#FFFFFF");
    public static readonly Color BorderColor = ColorTranslator.FromHtml("#E2E8F0");
    public static readonly Color HoverBackground = ColorTranslator.FromHtml("#F1F5F9");

    // ==========================================
    // 3. TEXT COLORS
    // ==========================================
    public static readonly Color TextPrimary = ColorTranslator.FromHtml("#1E293B");
    public static readonly Color TextSecondary = ColorTranslator.FromHtml("#64748B");
    public static readonly Color TextMuted = ColorTranslator.FromHtml("#94A3B8");
    public static readonly Color TextLight = ColorTranslator.FromHtml("#F8FAFC");

    // ==========================================
    // 4. STATUS COLORS (SPORTCHAIN STANDARD)
    // ==========================================
    public static readonly Color StatusAvailable = ColorTranslator.FromHtml("#10B981");      // Trống / Sẵn sàng (Xanh lá)
    public static readonly Color StatusPendingPayment = ColorTranslator.FromHtml("#F59E0B"); // Chờ cọc (Vàng hổ phách)
    public static readonly Color StatusConfirmed = ColorTranslator.FromHtml("#F97316");      // Đã cọc / Xác nhận (Cam)
    public static readonly Color StatusInUse = ColorTranslator.FromHtml("#EF4444");          // Đang thi đấu (Đỏ)
    public static readonly Color StatusMaintenance = ColorTranslator.FromHtml("#94A3B8");    // Bảo trì / Khóa (Xám)

    // Màu nền nhạt (Light Background) tương ứng cho Status Badge (Nền mờ dịu mắt)
    public static readonly Color StatusAvailableLight = ColorTranslator.FromHtml("#DCFCE7");
    public static readonly Color StatusPendingPaymentLight = ColorTranslator.FromHtml("#FEF3C7");
    public static readonly Color StatusConfirmedLight = ColorTranslator.FromHtml("#FFEDD5");
    public static readonly Color StatusInUseLight = ColorTranslator.FromHtml("#FEE2E2");
    public static readonly Color StatusMaintenanceLight = ColorTranslator.FromHtml("#F1F5F9");

    // ==========================================
    // 5. HELPER METHODS (MAPPING & UTILITIES)
    // ==========================================

    /// <summary>
    /// Lấy màu đại diện trạng thái theo tên chuỗi VisualStatus từ DTO
    /// </summary>
    public static Color GetStatusColor(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return StatusAvailable;

        return status.Trim().ToLowerInvariant() switch
        {
            "available" or "trong" or "santrong" => StatusAvailable,
            "pendingpayment" or "chococ" or "cho" => StatusPendingPayment,
            "confirmed" or "daxacnhan" or "daco" => StatusConfirmed,
            "inuse" or "dangthidau" or "dangda" => StatusInUse,
            "maintenance" or "baotri" or "khoa" => StatusMaintenance,
            "cancelled" or "dahuy" => ColorTranslator.FromHtml("#64748B"),
            _ => Primary
        };
    }

    /// <summary>
    /// Lấy màu nền nhạt (Soft Light Background) phục vụ cho Status Badge
    /// </summary>
    public static Color GetStatusLightColor(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return StatusAvailableLight;

        return status.Trim().ToLowerInvariant() switch
        {
            "available" or "trong" or "santrong" => StatusAvailableLight,
            "pendingpayment" or "chococ" or "cho" => StatusPendingPaymentLight,
            "confirmed" or "daxacnhan" or "daco" => StatusConfirmedLight,
            "inuse" or "dangthidau" or "dangda" => StatusInUseLight,
            "maintenance" or "baotri" or "khoa" => StatusMaintenanceLight,
            "cancelled" or "dahuy" => ColorTranslator.FromHtml("#F1F5F9"),
            _ => PrimaryLight
        };
    }

    /// <summary>
    /// Chuyển đổi mã Hex HTML sang đối tượng Color an toàn
    /// </summary>
    public static Color FromHex(string hex, Color defaultColor)
    {
        try
        {
            return ColorTranslator.FromHtml(hex);
        }
        catch
        {
            return defaultColor;
        }
    }
}
