using System.ComponentModel;
using System.Drawing;
using BTLtruc_quan.WinForms.Controls.Common;

namespace BTLtruc_quan.WinForms.Controls.Inputs;

/// <summary>
/// Hộp nhập liệu phẳng bo góc (Modern Flat TextBox) có hỗ trợ Placeholder Text,
/// hiệu ứng Focus viền đổi màu và tự động thích ứng với tỉ lệ màn hình High DPI.
/// </summary>
[DefaultEvent("TextChanged")]
public class ModernTextBox : UserControl
{
    protected TextBox _innerTextBox;
    private Color _borderColor = UITheme.BorderColor;
    private Color _borderFocusColor = UITheme.Primary;
    private int _borderSize = 1;
    private int _borderRadius = 8;
    private string _placeholderText = "";
    private Color _placeholderColor = UITheme.TextMuted;
    private bool _isFocused = false;
    private bool _isPlaceholder = false;

    public new event EventHandler? TextChanged;

    public ModernTextBox()
    {
        _innerTextBox = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            BackColor = Color.White
        };

        _innerTextBox.Enter += InnerTextBox_Enter;
        _innerTextBox.Leave += InnerTextBox_Leave;
        _innerTextBox.TextChanged += InnerTextBox_TextChanged;

        Padding = new Padding(10, 7, 10, 7);
        BackColor = Color.White;
        ForeColor = UITheme.TextPrimary;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        Size = new Size(250, 36);

        Controls.Add(_innerTextBox);
    }

    [Category("Appearance")]
    [Description("Màu đường viền mặc định")]
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
    [Description("Màu đường viền khi đang Focus nhập liệu")]
    public Color BorderFocusColor
    {
        get => _borderFocusColor;
        set
        {
            _borderFocusColor = value;
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue(1)]
    [Description("Độ dày đường viền (pixel)")]
    public int BorderSize
    {
        get => _borderSize;
        set
        {
            _borderSize = Math.Max(1, value);
            Invalidate();
        }
    }

    [Category("Appearance")]
    [DefaultValue(8)]
    [Description("Bán kính bo cong 4 góc (pixel)")]
    public int BorderRadius
    {
        get => _borderRadius;
        set
        {
            _borderRadius = Math.Max(0, value);
            Invalidate();
        }
    }

    [Category("Appearance")]
    [Description("Chuỗi văn bản gợi ý mờ (Placeholder Text)")]
    public string PlaceholderText
    {
        get => _placeholderText;
        set
        {
            _placeholderText = value;
            if (string.IsNullOrEmpty(_innerTextBox.Text))
            {
                SetPlaceholder();
            }
        }
    }

    [Category("Appearance")]
    [Description("Màu hiển thị của Placeholder Text")]
    public Color PlaceholderColor
    {
        get => _placeholderColor;
        set
        {
            _placeholderColor = value;
            if (_isPlaceholder)
            {
                _innerTextBox.ForeColor = value;
            }
        }
    }

    [Category("Appearance")]
    [Description("Ký tự ẩn mật khẩu (PasswordChar)")]
    public char PasswordChar
    {
        get => _innerTextBox.PasswordChar;
        set
        {
            if (!_isPlaceholder)
            {
                _innerTextBox.PasswordChar = value;
            }
        }
    }

    [Category("Behavior")]
    [Description("Cho phép nhập nhiều dòng (Multiline)")]
    public bool Multiline
    {
        get => _innerTextBox.Multiline;
        set
        {
            _innerTextBox.Multiline = value;
            UpdateControlHeight();
        }
    }

    [Category("Behavior")]
    [Description("Chỉ cho phép đọc (ReadOnly)")]
    public bool ReadOnly
    {
        get => _innerTextBox.ReadOnly;
        set => _innerTextBox.ReadOnly = value;
    }

    [Category("Appearance")]
    [Description("Nội dung chuỗi nhập liệu của TextBox")]
    public string Texts
    {
        get => _isPlaceholder ? "" : _innerTextBox.Text;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                SetPlaceholder();
            }
            else
            {
                RemovePlaceholder();
                _innerTextBox.Text = value;
            }
        }
    }

    private void SetPlaceholder()
    {
        if (string.IsNullOrEmpty(_placeholderText)) return;
        _isPlaceholder = true;
        _innerTextBox.Text = _placeholderText;
        _innerTextBox.ForeColor = _placeholderColor;
        if (PasswordChar != '\0')
        {
            _innerTextBox.UseSystemPasswordChar = false;
        }
    }

    private void RemovePlaceholder()
    {
        if (!_isPlaceholder) return;
        _isPlaceholder = false;
        _innerTextBox.Text = "";
        _innerTextBox.ForeColor = ForeColor;
        if (PasswordChar != '\0')
        {
            _innerTextBox.UseSystemPasswordChar = true;
        }
    }

    private void InnerTextBox_Enter(object? sender, EventArgs e)
    {
        _isFocused = true;
        if (_isPlaceholder)
        {
            RemovePlaceholder();
        }
        Invalidate();
    }

    private void InnerTextBox_Leave(object? sender, EventArgs e)
    {
        _isFocused = false;
        if (string.IsNullOrWhiteSpace(_innerTextBox.Text))
        {
            SetPlaceholder();
        }
        Invalidate();
    }

    private void InnerTextBox_TextChanged(object? sender, EventArgs e)
    {
        TextChanged?.Invoke(this, e);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateControlHeight();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateControlHeight();
        if (string.IsNullOrEmpty(_innerTextBox.Text))
        {
            SetPlaceholder();
        }
    }

    protected virtual void UpdateControlHeight()
    {
        if (_innerTextBox == null) return;

        if (!_innerTextBox.Multiline)
        {
            float factor = DpiHelper.GetScaleFactor(this);
            int txtHeight = TextRenderer.MeasureText("Text", Font).Height + 1;
            int paddingVertical = DpiHelper.Scale(14, factor);
            _innerTextBox.Multiline = true;
            _innerTextBox.MinimumSize = new Size(0, txtHeight);
            _innerTextBox.Size = new Size(Width - Padding.Horizontal, txtHeight);
            _innerTextBox.Multiline = false;

            Height = _innerTextBox.Height + paddingVertical;
        }

        _innerTextBox.Location = new Point(Padding.Left, (Height - _innerTextBox.Height) / 2);
        _innerTextBox.Width = Width - Padding.Horizontal;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        GraphicsHelper.ApplyHighQuality(g);

        float factor = DpiHelper.GetScaleFactor(this);
        int scaledRadius = DpiHelper.ScaleRadius(_borderRadius, factor);
        int scaledBorderSize = Math.Max(1, DpiHelper.Scale(_borderSize, factor));
        var bounds = new Rectangle(0, 0, Width - 1, Height - 1);

        // 1. Tô nền bo góc
        using (var bgBrush = new SolidBrush(BackColor))
        {
            GraphicsHelper.FillRoundedRectangle(g, bgBrush, bounds, scaledRadius);
        }

        // 2. Vẽ viền bo góc (màu viền phụ thuộc trạng thái Focus)
        Color activeBorderColor = _isFocused ? _borderFocusColor : _borderColor;
        using (var borderPen = new Pen(activeBorderColor, scaledBorderSize))
        {
            GraphicsHelper.DrawRoundedRectangle(g, borderPen, bounds, scaledRadius);
        }
    }
}
