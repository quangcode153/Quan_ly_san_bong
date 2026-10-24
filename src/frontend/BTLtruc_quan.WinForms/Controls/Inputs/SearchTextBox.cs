using System.ComponentModel;
using System.Drawing;
using BTLtruc_quan.WinForms.Controls.Common;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Controls.Inputs;

/// <summary>
/// Hộp tìm kiếm hiện đại (Modern Search TextBox) tích hợp icon kính lúp bên trái,
/// nút xóa nhanh (Clear) bên phải và sự kiện kích hoạt tìm kiếm khi gõ Enter.
/// </summary>
[DefaultEvent("SearchSubmitted")]
public class SearchTextBox : UserControl
{
    private readonly TextBox _innerTextBox;
    private readonly IconPictureBox _searchIcon;
    private readonly IconPictureBox _clearIcon;

    private Color _borderColor = UITheme.BorderColor;
    private Color _borderFocusColor = UITheme.Primary;
    private int _borderSize = 1;
    private int _borderRadius = 8;
    private string _placeholderText = "Tìm kiếm nhanh...";
    private Color _placeholderColor = UITheme.TextMuted;
    private bool _isFocused = false;
    private bool _isPlaceholder = false;

    public event EventHandler<string>? SearchSubmitted;
    public event EventHandler? SearchCleared;
    public new event EventHandler? TextChanged;

    public SearchTextBox()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

        DoubleBuffered = true;
        BackColor = Color.White;
        ForeColor = UITheme.TextPrimary;
        // 1. Icon tìm kiếm (bên trái)
        _searchIcon = new IconPictureBox
        {
            IconChar = IconChar.MagnifyingGlass,
            IconColor = UITheme.TextSecondary,
            IconSize = 16,
            BackColor = Color.Transparent,
            SizeMode = PictureBoxSizeMode.CenterImage,
            Cursor = Cursors.Hand,
            Size = new Size(24, 24)
        };
        _searchIcon.Click += (s, e) => OnSearchSubmitted();

        // 2. Icon xóa nhanh (bên phải, mặc định ẩn)
        _clearIcon = new IconPictureBox
        {
            IconChar = IconChar.Xmark,
            IconColor = UITheme.TextMuted,
            IconSize = 14,
            BackColor = Color.Transparent,
            SizeMode = PictureBoxSizeMode.CenterImage,
            Cursor = Cursors.Hand,
            Size = new Size(22, 22),
            Visible = false
        };
        _clearIcon.MouseEnter += (s, e) => _clearIcon.IconColor = UITheme.StatusInUse;
        _clearIcon.MouseLeave += (s, e) => _clearIcon.IconColor = UITheme.TextMuted;
        _clearIcon.Click += ClearIcon_Click;

        // 3. Inner TextBox
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
        _innerTextBox.KeyDown += InnerTextBox_KeyDown;

        Controls.Add(_searchIcon);
        Controls.Add(_innerTextBox);
        Controls.Add(_clearIcon);

        Size = new Size(260, 38);
        SetPlaceholder();
        LayoutInnerControls();
    }

    [Category("Appearance")]
    [Description("Màu viền mặc định")]
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [Description("Màu viền khi đang focus")]
    public Color BorderFocusColor
    {
        get => _borderFocusColor;
        set { _borderFocusColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(1)]
    [Description("Độ dày đường viền (pixel)")]
    public int BorderSize
    {
        get => _borderSize;
        set { _borderSize = Math.Max(1, value); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(8)]
    [Description("Bán kính bo cong 4 góc (pixel)")]
    public int BorderRadius
    {
        get => _borderRadius;
        set { _borderRadius = Math.Max(0, value); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue("Tìm kiếm nhanh...")]
    [Description("Chuỗi văn bản gợi ý mờ (Placeholder Text)")]
    public string PlaceholderText
    {
        get => _placeholderText;
        set
        {
            _placeholderText = value;
            if (string.IsNullOrEmpty(_innerTextBox.Text) || _isPlaceholder)
            {
                SetPlaceholder();
            }
        }
    }

    [Category("Appearance")]
    [Description("Nội dung chuỗi tìm kiếm")]
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
        _isPlaceholder = true;
        _innerTextBox.Text = _placeholderText;
        _innerTextBox.ForeColor = _placeholderColor;
        _clearIcon.Visible = false;
    }

    private void RemovePlaceholder()
    {
        _isPlaceholder = false;
        _innerTextBox.Text = "";
        _innerTextBox.ForeColor = ForeColor;
    }

    private void InnerTextBox_Enter(object? sender, EventArgs e)
    {
        _isFocused = true;
        _searchIcon.IconColor = _borderFocusColor;
        if (_isPlaceholder)
        {
            RemovePlaceholder();
        }
        Invalidate();
    }

    private void InnerTextBox_Leave(object? sender, EventArgs e)
    {
        _isFocused = false;
        _searchIcon.IconColor = UITheme.TextSecondary;
        if (string.IsNullOrWhiteSpace(_innerTextBox.Text))
        {
            SetPlaceholder();
        }
        Invalidate();
    }

    private void InnerTextBox_TextChanged(object? sender, EventArgs e)
    {
        if (!_isPlaceholder)
        {
            _clearIcon.Visible = !string.IsNullOrEmpty(_innerTextBox.Text);
            TextChanged?.Invoke(this, e);
        }
    }

    private void InnerTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            OnSearchSubmitted();
        }
    }

    private void ClearIcon_Click(object? sender, EventArgs e)
    {
        _innerTextBox.Clear();
        SetPlaceholder();
        SearchCleared?.Invoke(this, EventArgs.Empty);
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnSearchSubmitted()
    {
        string query = Texts.Trim();
        SearchSubmitted?.Invoke(this, query);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        LayoutInnerControls();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LayoutInnerControls();
    }

    private void LayoutInnerControls()
    {
        if (_searchIcon == null || _clearIcon == null || _innerTextBox == null) return;

        float factor = DpiHelper.GetScaleFactor(this);

        int iconPaddingLeft = DpiHelper.Scale(10, factor);
        int iconPaddingRight = DpiHelper.Scale(10, factor);
        int iconSize = DpiHelper.Scale(18, factor);

        _searchIcon.IconSize = iconSize;
        _searchIcon.Size = new Size(iconSize + 4, iconSize + 4);
        _searchIcon.Location = new Point(iconPaddingLeft, (Height - _searchIcon.Height) / 2);

        int clearSize = DpiHelper.Scale(16, factor);
        _clearIcon.IconSize = clearSize;
        _clearIcon.Size = new Size(clearSize + 4, clearSize + 4);
        _clearIcon.Location = new Point(Width - _clearIcon.Width - iconPaddingRight, (Height - _clearIcon.Height) / 2);

        int txtLeft = _searchIcon.Right + DpiHelper.Scale(6, factor);
        int txtRight = _clearIcon.Left - DpiHelper.Scale(4, factor);
        int txtWidth = Math.Max(20, txtRight - txtLeft);

        _innerTextBox.Location = new Point(txtLeft, (Height - _innerTextBox.Height) / 2);
        _innerTextBox.Width = txtWidth;
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

        // 2. Vẽ viền bo góc
        Color activeBorderColor = _isFocused ? _borderFocusColor : _borderColor;
        using (var borderPen = new Pen(activeBorderColor, scaledBorderSize))
        {
            GraphicsHelper.DrawRoundedRectangle(g, borderPen, bounds, scaledRadius);
        }
    }
}
