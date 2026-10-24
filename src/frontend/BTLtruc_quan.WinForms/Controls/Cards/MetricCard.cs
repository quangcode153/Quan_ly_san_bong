using System.ComponentModel;
using System.Drawing;
using BTLtruc_quan.WinForms.Controls.Common;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Controls.Cards;

/// <summary>
/// Thẻ thống kê chỉ số Dashboard (Metric Card) phẳng, bo góc mềm mại.
/// Trực quan hóa số liệu kinh doanh, sân thi đấu, doanh thu kèm icon FontAwesome vector.
/// </summary>
public class MetricCard : Control
{
    private string _title = "TIÊU ĐỀ CHỈ SỐ";
    private string _value = "0";
    private string _trendText = "+0% so với hôm qua";
    private Color _trendColor = UITheme.Primary;
    private IconChar _icon = IconChar.ChartSimple;
    private Color _iconColor = UITheme.Primary;
    private Color _iconBackgroundColor = UITheme.PrimaryLight;
    private Color _cardColor = UITheme.CardBackground;
    private Color _borderColor = UITheme.BorderColor;
    private int _cornerRadius = 12;
    private float _borderSize = 1f;

    public MetricCard()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);

        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Size = new Size(240, 110);
        Font = new Font("Segoe UI", 9f, FontStyle.Regular);
    }

    [Category("Appearance")]
    [DefaultValue("TIÊU ĐỀ CHỈ SỐ")]
    [Description("Tiêu đề của chỉ số thống kê")]
    public string Title
    {
        get => _title;
        set { _title = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue("0")]
    [Description("Giá trị số liệu nổi bật")]
    public string Value
    {
        get => _value;
        set { _value = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue("+0% so với hôm qua")]
    [Description("Dòng ghi chú xu hướng hoặc phần trăm")]
    public string TrendText
    {
        get => _trendText;
        set { _trendText = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Màu sắc của dòng xu hướng")]
    public Color TrendColor
    {
        get => _trendColor;
        set { _trendColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(IconChar.ChartSimple)]
    [Description("Biểu tượng FontAwesome đại diện cho thẻ")]
    public IconChar Icon
    {
        get => _icon;
        set { _icon = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Màu của biểu tượng Icon")]
    public Color IconColor
    {
        get => _iconColor;
        set { _iconColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Màu nền của vùng tròn chứa Icon")]
    public Color IconBackgroundColor
    {
        get => _iconBackgroundColor;
        set { _iconBackgroundColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Màu nền của thẻ Card")]
    public Color CardColor
    {
        get => _cardColor;
        set { _cardColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Màu đường viền thẻ")]
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(12)]
    [Description("Bán kính bo cong 4 góc của thẻ (pixel)")]
    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = Math.Max(0, value); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(1f)]
    [Description("Độ dày đường viền (pixel)")]
    public float BorderSize
    {
        get => _borderSize;
        set { _borderSize = Math.Max(0, value); Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        GraphicsHelper.ApplyHighQuality(g);

        float factor = DpiHelper.GetScaleFactor(this);
        int scaledRadius = DpiHelper.ScaleRadius(_cornerRadius, factor);
        var bounds = new Rectangle(0, 0, Width - 1, Height - 1);

        // 1. Vẽ nền thẻ bo góc
        using (var cardBrush = new SolidBrush(_cardColor))
        {
            GraphicsHelper.FillRoundedRectangle(g, cardBrush, bounds, scaledRadius);
        }

        // 2. Vẽ viền thẻ
        if (_borderSize > 0 && _borderColor != Color.Transparent)
        {
            float scaledBorderSize = Math.Max(1f, DpiHelper.Scale((int)_borderSize, factor));
            using var borderPen = new Pen(_borderColor, scaledBorderSize);
            GraphicsHelper.DrawRoundedRectangle(g, borderPen, bounds, scaledRadius);
        }

        // 3. Tính toán vị trí và vẽ Icon box (hình tròn bên phải hoặc trái)
        int padding = DpiHelper.Scale(16, factor);
        int iconBoxSize = DpiHelper.Scale(46, factor);
        int iconBoxX = Width - padding - iconBoxSize;
        int iconBoxY = padding;

        var iconBoxRect = new Rectangle(iconBoxX, iconBoxY, iconBoxSize, iconBoxSize);

        // Vẽ hình tròn nền của icon
        using (var iconBgBrush = new SolidBrush(_iconBackgroundColor))
        {
            g.FillEllipse(iconBgBrush, iconBoxRect);
        }

        // Vẽ FontAwesome Icon vào tâm hình tròn
        int iconPixelSize = DpiHelper.Scale(22, factor);
        try
        {
            using var iconBitmap = _icon.ToBitmap(_iconColor, iconPixelSize);
            if (iconBitmap != null)
            {
                int iconX = iconBoxX + (iconBoxSize - iconBitmap.Width) / 2;
                int iconY = iconBoxY + (iconBoxSize - iconBitmap.Height) / 2;
                g.DrawImage(iconBitmap, iconX, iconY);
            }
        }
        catch
        {
            // Dự phòng nếu font chưa sẵn sàng
        }

        // 4. Vẽ Tiêu đề (Title)
        int textLeft = padding;
        int textTop = padding;
        int maxTextWidth = iconBoxX - textLeft - DpiHelper.Scale(6, factor);

        using (var titleFont = new Font("Segoe UI", Math.Max(7f, 8f * factor), FontStyle.Bold))
        using (var titleBrush = new SolidBrush(UITheme.TextSecondary))
        {
            var titleRect = new RectangleF(textLeft, textTop, maxTextWidth, DpiHelper.Scale(18, factor));
            g.DrawString(_title.ToUpper(), titleFont, titleBrush, titleRect);
        }

        // 5. Vẽ Giá trị nổi bật (Value)
        int valueTop = textTop + DpiHelper.Scale(17, factor);
        using (var valFont = new Font("Segoe UI", Math.Max(12f, 15.5f * factor), FontStyle.Bold))
        using (var valBrush = new SolidBrush(UITheme.TextPrimary))
        {
            var valRect = new RectangleF(textLeft, valueTop, maxTextWidth + DpiHelper.Scale(6, factor), DpiHelper.Scale(34, factor));
            g.DrawString(_value, valFont, valBrush, valRect);
        }

        // 6. Vẽ Dòng xu hướng / chú thích (TrendText)
        if (!string.IsNullOrWhiteSpace(_trendText))
        {
            int trendTop = Height - padding - DpiHelper.Scale(16, factor);
            using (var trendFont = new Font("Segoe UI", Math.Max(7.5f, 8.5f * factor), FontStyle.Regular))
            using (var trendBrush = new SolidBrush(_trendColor))
            {
                var trendRect = new RectangleF(textLeft, trendTop, Width - (padding * 2), DpiHelper.Scale(18, factor));
                g.DrawString(_trendText, trendFont, trendBrush, trendRect);
            }
        }
    }
}
