using System.ComponentModel;
using System.Drawing;
using BTLtruc_quan.WinForms.Controls.Common;

namespace BTLtruc_quan.WinForms.Controls.Badges;

/// <summary>
/// Huy hiệu trạng thái bo tròn (Pill-shaped Status Badge) chuyên nghiệp cho SportChain.
/// Tự động tính toán kích thước, font và padding theo tỷ lệ High DPI.
/// </summary>
[DefaultProperty("Text")]
public class StatusBadge : Control
{
    private string? _badgeStatus = "Available";
    private Color _badgeColor = UITheme.StatusAvailable;
    private Color _badgeLightColor = UITheme.StatusAvailableLight;
    private Color? _customTextColor = null;
    private Color _borderColor = Color.Transparent;
    private float _borderSize = 0f;
    private int _cornerRadius = -1; // -1: Tự động bo tròn kiểu viên thuốc (Pill Shape: Height / 2)
    private bool _showDot = true;
    private bool _autoBadgeSize = true;

    public StatusBadge()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);

        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        Text = "Sẵn sàng";
        Size = new Size(100, 28);
        UpdateColors();
    }

    [Category("Appearance")]
    [Description("Chuỗi định danh trạng thái (Available, PendingPayment, Confirmed, InUse, Maintenance,...)")]
    public string? BadgeStatus
    {
        get => _badgeStatus;
        set
        {
            _badgeStatus = value;
            UpdateColors();
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu đại diện chính của trạng thái (dùng cho dot, viền và chữ)")]
    public Color BadgeColor
    {
        get => _badgeColor;
        set
        {
            _badgeColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu nền nhạt (Soft Light Background) của badge")]
    public Color BadgeLightColor
    {
        get => _badgeLightColor;
        set
        {
            _badgeLightColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu chữ tùy biến. Nếu để null, màu chữ sẽ tự động dùng BadgeColor")]
    public Color? CustomTextColor
    {
        get => _customTextColor;
        set
        {
            _customTextColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu đường viền bao quanh")]
    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            _borderColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Độ dày đường viền bao quanh (pixel)")]
    public float BorderSize
    {
        get => _borderSize;
        set
        {
            _borderSize = Math.Max(0, value);
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Bán kính bo góc. Giá trị -1 sẽ tự động bo tròn nửa chiều cao (Pill Shape)")]
    public int CornerRadius
    {
        get => _cornerRadius;
        set
        {
            _cornerRadius = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Bật/tắt chấm tròn trạng thái (Status Dot) bên trái text")]
    public bool ShowDot
    {
        get => _showDot;
        set
        {
            _showDot = value;
            if (_autoBadgeSize) RecalculateSize();
            Invalidate();
        }
    }

    [Category("Behavior")]
    [Description("Tự động co giãn kích thước theo nội dung văn bản và tỷ lệ DPI")]
    public bool AutoBadgeSize
    {
        get => _autoBadgeSize;
        set
        {
            _autoBadgeSize = value;
            if (value) RecalculateSize();
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [System.Diagnostics.CodeAnalysis.AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            if (_autoBadgeSize) RecalculateSize();
            Invalidate();
        }
    }

    /// <summary>
    /// Gán trạng thái và tự động cập nhật màu sắc + văn bản mặc định
    /// </summary>
    public void SetStatus(string? status, string? customText = null)
    {
        _badgeStatus = status;
        UpdateColors();

        if (!string.IsNullOrWhiteSpace(customText))
        {
            Text = customText;
        }
        else
        {
            Text = GetDefaultStatusText(status);
        }

        if (_autoBadgeSize) RecalculateSize();
        Invalidate();
    }

    private void UpdateColors()
    {
        _badgeColor = UITheme.GetStatusColor(_badgeStatus);
        _badgeLightColor = UITheme.GetStatusLightColor(_badgeStatus);
    }

    private static string GetDefaultStatusText(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return "Sẵn sàng";

        return status.Trim().ToLowerInvariant() switch
        {
            "available" or "trong" => "Sẵn sàng",
            "pendingpayment" or "chococ" => "Chờ đặt cọc",
            "confirmed" or "daxacnhan" => "Đã xác nhận",
            "inuse" or "dangthidau" => "Đang thi đấu",
            "maintenance" or "baotri" => "Bảo trì",
            "completed" or "hoanthanh" => "Hoàn thành",
            "cancelled" or "dahuy" => "Đã hủy",
            _ => status
        };
    }

    private void RecalculateSize()
    {
        if (string.IsNullOrEmpty(Text)) return;

        float factor = DpiHelper.GetScaleFactor(this);
        using var g = CreateGraphics();
        var textSize = g.MeasureString(Text, Font);

        int dotSpacing = _showDot ? DpiHelper.Scale(16, factor) : 0;
        int horizontalPadding = DpiHelper.Scale(24, factor);
        int verticalPadding = DpiHelper.Scale(10, factor);

        int newWidth = (int)Math.Ceiling(textSize.Width) + dotSpacing + horizontalPadding;
        int newHeight = Math.Max(DpiHelper.Scale(26, factor), (int)Math.Ceiling(textSize.Height) + verticalPadding);

        Size = new Size(newWidth, newHeight);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        if (_autoBadgeSize) RecalculateSize();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        GraphicsHelper.ApplyHighQuality(g);

        float factor = DpiHelper.GetScaleFactor(this);
        int radius = _cornerRadius < 0 ? Height / 2 : DpiHelper.ScaleRadius(_cornerRadius, factor);
        var bounds = new Rectangle(0, 0, Width, Height);

        // 1. Tô màu nền nhạt bo góc (Light Background)
        using (var brush = new SolidBrush(_badgeLightColor))
        {
            GraphicsHelper.FillRoundedRectangle(g, brush, bounds, radius);
        }

        // 2. Vẽ đường viền (nếu có cấu hình)
        if (_borderSize > 0 && _borderColor != Color.Transparent)
        {
            using var pen = new Pen(_borderColor, DpiHelper.Scale((int)_borderSize, factor));
            GraphicsHelper.DrawRoundedRectangle(g, pen, bounds, radius);
        }

        // 3. Tính toán vị trí vẽ Dot và Text
        var textSize = g.MeasureString(Text, Font);
        float dotSize = DpiHelper.Scale(8, factor);
        float spacing = DpiHelper.Scale(6, factor);

        float totalContentWidth = textSize.Width + (_showDot ? dotSize + spacing : 0);
        float startX = (Width - totalContentWidth) / 2f;
        float startY = (Height - textSize.Height) / 2f;

        // Vẽ Status Dot
        if (_showDot)
        {
            float dotY = (Height - dotSize) / 2f;
            using var dotBrush = new SolidBrush(_badgeColor);
            g.FillEllipse(dotBrush, startX, dotY, dotSize, dotSize);
            startX += dotSize + spacing;
        }

        // Vẽ Text
        Color textColor = _customTextColor ?? _badgeColor;
        using var textBrush = new SolidBrush(textColor);
        g.DrawString(Text, Font, textBrush, startX, startY);
    }
}
