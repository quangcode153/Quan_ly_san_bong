using System.Drawing;

namespace BTLtruc_quan.WinForms.Controls.Common;

/// <summary>
/// Lớp tiện ích cung cấp các phép tính toán tỉ lệ co giãn High DPI (DPI-Aware Scaling),
/// đảm bảo giao diện hiển thị sắc nét, không vỡ layout trên các màn hình có tỉ lệ phóng to khác nhau (100% - 200%).
/// </summary>
public static class DpiHelper
{
    /// <summary>
    /// Mức DPI tiêu chuẩn của hệ điều hành Windows (tương ứng 100% Display Scaling)
    /// </summary>
    public const float StandardDpi = 96.0f;

    /// <summary>
    /// Lấy hệ số tỉ lệ co giãn (Scale Factor) hiện tại của một Control hoặc Form
    /// Ví dụ: 96 DPI -> 1.0f; 120 DPI (125%) -> 1.25f; 144 DPI (150%) -> 1.5f; 192 DPI (200%) -> 2.0f.
    /// </summary>
    public static float GetScaleFactor(Control? control)
    {
        if (control != null && control.IsHandleCreated)
        {
            return control.DeviceDpi / StandardDpi;
        }

        // Trường hợp control chưa tạo handle, thử lấy từ màn hình chính
        using var g = Graphics.FromHwnd(IntPtr.Zero);
        return g.DpiX / StandardDpi;
    }

    /// <summary>
    /// Tính toán giá trị nguyên (int) theo hệ số tỉ lệ phóng to
    /// </summary>
    public static int Scale(int value, float factor)
    {
        return (int)Math.Round(value * factor);
    }

    /// <summary>
    /// Tính toán kích thước (Size) theo hệ số tỉ lệ phóng to
    /// </summary>
    public static Size ScaleSize(Size size, float factor)
    {
        return new Size(
            Scale(size.Width, factor),
            Scale(size.Height, factor)
        );
    }

    /// <summary>
    /// Tính toán tọa độ (Point) theo hệ số tỉ lệ phóng to
    /// </summary>
    public static Point ScalePoint(Point point, float factor)
    {
        return new Point(
            Scale(point.X, factor),
            Scale(point.Y, factor)
        );
    }

    /// <summary>
    /// Tính toán khoảng cách đệm (Padding) theo hệ số tỉ lệ phóng to
    /// </summary>
    public static Padding ScalePadding(Padding padding, float factor)
    {
        return new Padding(
            Scale(padding.Left, factor),
            Scale(padding.Top, factor),
            Scale(padding.Right, factor),
            Scale(padding.Bottom, factor)
        );
    }

    /// <summary>
    /// Tính toán bán kính bo góc (CornerRadius) theo hệ số tỉ lệ phóng to
    /// </summary>
    public static int ScaleRadius(int radius, float factor)
    {
        return Math.Max(1, Scale(radius, factor));
    }

    /// <summary>
    /// Tạo một Font mới dựa trên font gốc đã được scale theo hệ số tỉ lệ phóng to
    /// </summary>
    public static Font ScaleFont(Font originalFont, float factor)
    {
        if (originalFont == null)
            return SystemFonts.DefaultFont;

        float scaledSize = Math.Max(6f, originalFont.Size * factor);
        return new Font(originalFont.FontFamily, scaledSize, originalFont.Style, originalFont.Unit);
    }
}
