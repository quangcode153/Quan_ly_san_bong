using System.ComponentModel;
using System.Drawing;
using BTLtruc_quan.WinForms.Controls.Common;

namespace BTLtruc_quan.WinForms.Controls.Inputs;

/// <summary>
/// Công tắc bật/tắt (Toggle Switch) phẳng, hiện đại thay thế CheckBox mặc định.
/// Được vẽ bằng GDI+ khử răng cưa, tự động căn chỉnh và co giãn theo High DPI.
/// </summary>
[DefaultEvent("CheckedChanged")]
public class ToggleSwitch : Control
{
    private bool _checked = false;
    private Color _onColor = UITheme.Primary;
    private Color _offColor = ColorTranslator.FromHtml("#CBD5E1");
    private Color _thumbColor = Color.White;
    private Color _thumbBorderColor = ColorTranslator.FromHtml("#94A3B8");
    private bool _showText = false;
    private string _onText = "ON";
    private string _offText = "OFF";

    public event EventHandler? CheckedChanged;

    public ToggleSwitch()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);

        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        Size = new Size(50, 26);
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Trạng thái bật/tắt của công tắc")]
    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked != value)
            {
                _checked = value;
                OnCheckedChanged();
                Invalidate();
            }
        }
    }

    [Category("Appearance")]
    [Description("Màu thân công tắc khi ở trạng thái Bật (ON)")]
    public Color OnColor
    {
        get => _onColor;
        set
        {
            _onColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu thân công tắc khi ở trạng thái Tắt (OFF)")]
    public Color OffColor
    {
        get => _offColor;
        set
        {
            _offColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Màu núm trượt tròn (Thumb)")]
    public Color ThumbColor
    {
        get => _thumbColor;
        set
        {
            _thumbColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("Hiển thị chữ ON/OFF bên trong thân công tắc")]
    public bool ShowText
    {
        get => _showText;
        set
        {
            _showText = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue("ON")]
    public string OnText
    {
        get => _onText;
        set
        {
            _onText = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue("OFF")]
    public string OffText
    {
        get => _offText;
        set
        {
            _offText = value;
            Invalidate();
        }
    }

    protected virtual void OnCheckedChanged()
    {
        CheckedChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Checked = !Checked;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Space)
        {
            Checked = !Checked;
            e.Handled = true;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        GraphicsHelper.ApplyHighQuality(g);

        float factor = DpiHelper.GetScaleFactor(this);

        // Tính toán kích thước thân pill
        int pillRadius = Height / 2;
        var bounds = new Rectangle(0, 0, Width - 1, Height - 1);

        // 1. Vẽ thân công tắc bo tròn
        Color currentBodyColor = _checked ? _onColor : _offColor;
        using (var brush = new SolidBrush(currentBodyColor))
        {
            GraphicsHelper.FillRoundedRectangle(g, brush, bounds, pillRadius);
        }

        // 2. Vẽ text ON / OFF nếu được bật
        if (_showText)
        {
            string statusText = _checked ? _onText : _offText;
            using var font = new Font("Segoe UI", Math.Max(7f, 8f * factor), FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.White);
            var textSize = g.MeasureString(statusText, font);

            float textX = _checked
                ? pillRadius / 2f
                : Width - textSize.Width - (pillRadius / 2f);
            float textY = (Height - textSize.Height) / 2f;

            g.DrawString(statusText, font, textBrush, textX, textY);
        }

        // 3. Tính toán vị trí núm trượt tròn (Thumb)
        int margin = Math.Max(2, DpiHelper.Scale(3, factor));
        int thumbDiameter = Height - (margin * 2);

        int thumbX = _checked
            ? Width - thumbDiameter - margin - 1
            : margin;
        int thumbY = margin;

        var thumbBounds = new Rectangle(thumbX, thumbY, thumbDiameter, thumbDiameter);

        // Đổ bóng nhẹ cho núm trượt
        using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
        {
            g.FillEllipse(shadowBrush, thumbX + 1, thumbY + 1, thumbDiameter, thumbDiameter);
        }

        // Vẽ núm tròn
        using (var thumbBrush = new SolidBrush(_thumbColor))
        {
            g.FillEllipse(thumbBrush, thumbBounds);
        }

        // Viền mảnh tinh tế cho núm
        using (var thumbBorderPen = new Pen(_thumbBorderColor, 1f))
        {
            g.DrawEllipse(thumbBorderPen, thumbBounds);
        }
    }
}
