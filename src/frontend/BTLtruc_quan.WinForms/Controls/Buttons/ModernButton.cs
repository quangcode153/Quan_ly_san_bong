using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using BTLtruc_quan.WinForms.Controls.Common;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Controls.Buttons;

/// <summary>
/// Nút bấm phẳng hiện đại (Modern Flat Button) tích hợp vector icon từ FontAwesome.Sharp.
/// Hỗ trợ bo tròn góc mềm mại, hiệu ứng Hover/Pressed và tương thích hoàn hảo với High DPI.
/// </summary>
public class ModernButton : IconButton
{
    private int _cornerRadius = 8;
    private float _borderSize = 0f;
    private Color _borderColor = Color.Transparent;
    private Color _backgroundColor = UITheme.Primary;
    private Color _hoverColor = UITheme.PrimaryDark;
    private Color _pressedColor = ColorTranslator.FromHtml("#047857");
    private bool _isHovered = false;
    private bool _isPressed = false;

    public ModernButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseDownBackColor = Color.Transparent;
        FlatAppearance.MouseOverBackColor = Color.Transparent;
        BackColor = _backgroundColor;
        ForeColor = Color.White;
        IconColor = Color.White;
        IconSize = 18;
        Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
        Cursor = Cursors.Hand;
        Size = new Size(130, 38);
        TextImageRelation = TextImageRelation.ImageBeforeText;
        ImageAlign = ContentAlignment.MiddleCenter;
        TextAlign = ContentAlignment.MiddleCenter;
        UseMnemonic = false;
    }

    [Category("Appearance")]
    [DefaultValue(8)]
    [Description("Bán kính bo cong 4 góc (pixel)")]
    public int CornerRadius
    {
        get => _cornerRadius;
        set
        {
            _cornerRadius = Math.Max(0, value);
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue(0f)]
    [Description("Độ dày đường viền (pixel)")]
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
    [Description("Màu đường viền nút")]
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
    [Description("Màu nền mặc định của nút")]
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            BackColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu nền khi rê chuột vào (Hover)")]
    public Color HoverColor
    {
        get => _hoverColor;
        set
        {
            _hoverColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu nền khi bấm giữ chuột (Pressed)")]
    public Color PressedColor
    {
        get => _pressedColor;
        set
        {
            _pressedColor = value;
            Invalidate();
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        _isPressed = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        base.OnMouseDown(mevent);
        _isPressed = true;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        base.OnMouseUp(mevent);
        _isPressed = false;
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRegion();
    }

    private void UpdateRegion()
    {
        if (Width <= 0 || Height <= 0) return;

        float factor = DpiHelper.GetScaleFactor(this);
        int scaledRadius = DpiHelper.ScaleRadius(_cornerRadius, factor);

        if (scaledRadius > 1)
        {
            using var path = GraphicsHelper.GetRoundedRectanglePath(new Rectangle(0, 0, Width, Height), scaledRadius);
            Region = new Region(path);
        }
        else
        {
            Region = new Region(new Rectangle(0, 0, Width, Height));
        }
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        Graphics g = pevent.Graphics;
        GraphicsHelper.ApplyHighQuality(g);

        float factor = DpiHelper.GetScaleFactor(this);
        int scaledRadius = DpiHelper.ScaleRadius(_cornerRadius, factor);
        var bounds = new Rectangle(0, 0, Width, Height);

        // 1. Xác định màu nền tương ứng trạng thái
        Color currentColor = _isPressed
            ? _pressedColor
            : (_isHovered ? _hoverColor : _backgroundColor);

        // 2. Tô nền bo tròn góc
        using (var brush = new SolidBrush(currentColor))
        {
            GraphicsHelper.FillRoundedRectangle(g, brush, bounds, scaledRadius);
        }

        // 3. Gọi vẽ nội dung của IconButton (Icon & Text)
        base.OnPaint(pevent);

        // 4. Vẽ đường viền đè lên trên (nếu có)
        if (_borderSize > 0 && _borderColor != Color.Transparent)
        {
            float scaledBorderSize = DpiHelper.Scale((int)_borderSize, factor);
            using var pen = new Pen(_borderColor, Math.Max(1f, scaledBorderSize));
            GraphicsHelper.DrawRoundedRectangle(g, pen, bounds, scaledRadius);
        }
    }
}
