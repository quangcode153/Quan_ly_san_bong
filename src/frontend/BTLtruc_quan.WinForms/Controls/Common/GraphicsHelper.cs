using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace BTLtruc_quan.WinForms.Controls.Common;

/// <summary>
/// Lớp tiện ích cung cấp các hàm hỗ trợ vẽ đồ họa GDI+ chất lượng cao,
/// bo tròn góc và quản lý giải phóng tài nguyên bộ nhớ chặt chẽ.
/// </summary>
public static class GraphicsHelper
{
    /// <summary>
    /// Áp dụng cấu hình khử răng cưa và render đồ họa chất lượng cao nhất cho bề mặt vẽ
    /// </summary>
    public static void ApplyHighQuality(Graphics g)
    {
        if (g == null) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    }

    /// <summary>
    /// Tạo đối tượng GraphicsPath bo tròn 4 góc cho hình chữ nhật chuẩn xác.
    /// Có kiểm tra giới hạn bán kính an toàn để tránh lỗi hình học GDI+.
    /// LƯU Ý: Đối tượng trả về cần được bao bọc trong khối using(...) sau khi sử dụng!
    /// </summary>
    public static GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();

        // Kiểm tra an toàn: nếu kích thước không hợp lệ, trả về path rỗng
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return path;

        // Nếu bán kính <= 0, vẽ hình chữ nhật góc vuông bình thường
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        // Bán kính không được vượt quá một nửa chiều rộng hoặc chiều cao
        int maxRadius = Math.Min(bounds.Width, bounds.Height) / 2;
        if (radius > maxRadius)
        {
            radius = maxRadius;
        }

        int diameter = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

        // Góc trên - bên trái
        path.AddArc(arc, 180, 90);

        // Góc trên - bên phải
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        // Góc dưới - bên phải
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // Góc dưới - bên trái
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// Tô màu (Fill) hình chữ nhật bo góc với Brush cho trước, tự động quản lý GraphicsPath
    /// </summary>
    public static void FillRoundedRectangle(Graphics g, Brush brush, Rectangle bounds, int radius)
    {
        if (g == null || brush == null || bounds.Width <= 0 || bounds.Height <= 0)
            return;

        using var path = GetRoundedRectanglePath(bounds, radius);
        g.FillPath(brush, path);
    }

    /// <summary>
    /// Tô màu (Fill) hình chữ nhật bo góc với Color
    /// </summary>
    public static void FillRoundedRectangle(Graphics g, Color color, Rectangle bounds, int radius)
    {
        if (g == null || bounds.Width <= 0 || bounds.Height <= 0)
            return;

        using var brush = new SolidBrush(color);
        FillRoundedRectangle(g, brush, bounds, radius);
    }

    /// <summary>
    /// Vẽ đường viền (Draw) hình chữ nhật bo góc với Pen cho trước, tự động tính trừ viền để không bị cụt
    /// </summary>
    public static void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle bounds, int radius)
    {
        if (g == null || pen == null || bounds.Width <= 0 || bounds.Height <= 0)
            return;

        // Lùi tọa độ 1 pixel để đường viền nằm gọn bên trong ranh giới bounds
        var adjustedBounds = new Rectangle(
            bounds.X,
            bounds.Y,
            bounds.Width - Math.Max(1, (int)pen.Width),
            bounds.Height - Math.Max(1, (int)pen.Width)
        );

        using var path = GetRoundedRectanglePath(adjustedBounds, radius);
        g.DrawPath(pen, path);
    }

    /// <summary>
    /// Vẽ đường viền (Draw) hình chữ nhật bo góc với Color và độ dày viền
    /// </summary>
    public static void DrawRoundedRectangle(Graphics g, Color color, float borderSize, Rectangle bounds, int radius)
    {
        if (g == null || borderSize <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            return;

        using var pen = new Pen(color, borderSize);
        DrawRoundedRectangle(g, pen, bounds, radius);
    }
}
