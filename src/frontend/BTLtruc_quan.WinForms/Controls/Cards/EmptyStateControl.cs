using System.ComponentModel;
using System.Drawing;
using BTLtruc_quan.WinForms.Controls.Buttons;
using BTLtruc_quan.WinForms.Controls.Common;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Controls.Cards;

/// <summary>
/// Giao diện trạng thái rỗng (Empty State Control) chuyên nghiệp.
/// Hiển thị khi danh sách/bảng chưa có dữ liệu, không tìm thấy kết quả hoặc chưa có lịch đặt sân.
/// </summary>
[DefaultEvent("ActionClick")]
public class EmptyStateControl : UserControl
{
    private readonly IconPictureBox _iconPicture;
    private readonly Label _titleLabel;
    private readonly Label _descriptionLabel;
    private readonly ModernButton _actionButton;

    private IconChar _icon = IconChar.CalendarXmark;
    private Color _iconColor = UITheme.TextMuted;
    private bool _showActionButton = true;

    public event EventHandler? ActionClick;

    public EmptyStateControl()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer, true);

        DoubleBuffered = true;
        BackColor = Color.Transparent;

        // 1. Icon trung tâm
        _iconPicture = new IconPictureBox
        {
            IconChar = _icon,
            IconColor = _iconColor,
            IconSize = 56,
            BackColor = Color.Transparent,
            SizeMode = PictureBoxSizeMode.CenterImage,
            Size = new Size(64, 64)
        };

        // 2. Tiêu đề chính
        _titleLabel = new Label
        {
            Text = "Không có dữ liệu hiển thị",
            UseMnemonic = false,
            Font = new Font("Segoe UI Semibold", 12f, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            TextAlign = ContentAlignment.MiddleCenter,
            AutoSize = false,
            Height = 28
        };

        // 3. Mô tả phụ
        _descriptionLabel = new Label
        {
            Text = "Hiện tại chưa có thông tin nào phù hợp. Vui lòng thử tìm kiếm lại hoặc tạo mới dữ liệu.",
            UseMnemonic = false,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = UITheme.TextSecondary,
            TextAlign = ContentAlignment.TopCenter,
            AutoSize = false,
            Height = 44
        };

        // 4. Nút bấm hành động (CTA)
        _actionButton = new ModernButton
        {
            Text = "Tạo đơn đặt sân mới",
            IconChar = IconChar.Plus,
            IconColor = Color.White,
            IconSize = 16,
            CornerRadius = 8,
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            Size = new Size(180, 38)
        };
        _actionButton.Click += (s, e) => ActionClick?.Invoke(this, e);

        Controls.Add(_iconPicture);
        Controls.Add(_titleLabel);
        Controls.Add(_descriptionLabel);
        Controls.Add(_actionButton);

        Size = new Size(360, 260);
        RearrangeLayout();
    }

    [Category("Appearance")]
    [DefaultValue(IconChar.CalendarXmark)]
    [Description("Biểu tượng vector FontAwesome hiển thị ở giữa")]
    public IconChar Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            _iconPicture.IconChar = value;
        }
    }

    [Category("Appearance")]
    [Description("Màu sắc của biểu tượng")]
    public Color IconColor
    {
        get => _iconColor;
        set
        {
            _iconColor = value;
            _iconPicture.IconColor = value;
        }
    }

    [Category("Appearance")]
    [DefaultValue("Không có dữ liệu hiển thị")]
    [Description("Dòng tiêu đề chính")]
    public string Title
    {
        get => _titleLabel.Text;
        set => _titleLabel.Text = value;
    }

    [Category("Appearance")]
    [DefaultValue("Hiện tại chưa có thông tin nào phù hợp. Vui lòng thử tìm kiếm lại hoặc tạo mới dữ liệu.")]
    [Description("Dòng mô tả hướng dẫn chi tiết")]
    public string Description
    {
        get => _descriptionLabel.Text;
        set => _descriptionLabel.Text = value;
    }

    [Category("Appearance")]
    [DefaultValue("Tạo đơn đặt sân mới")]
    [Description("Nhãn chữ trên nút hành động")]
    public string ActionButtonText
    {
        get => _actionButton.Text;
        set => _actionButton.Text = value;
    }

    [Category("Appearance")]
    [DefaultValue(IconChar.Plus)]
    [Description("Icon trên nút hành động")]
    public IconChar ActionButtonIcon
    {
        get => _actionButton.IconChar;
        set => _actionButton.IconChar = value;
    }

    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Cho phép ẩn/hiện nút hành động bên dưới")]
    public bool ShowActionButton
    {
        get => _showActionButton;
        set
        {
            _showActionButton = value;
            _actionButton.Visible = value;
            RearrangeLayout();
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        RearrangeLayout();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RearrangeLayout();
    }

    private void RearrangeLayout()
    {
        if (Width <= 0 || Height <= 0) return;
        if (_iconPicture == null || _titleLabel == null || _descriptionLabel == null || _actionButton == null) return;

        float factor = DpiHelper.GetScaleFactor(this);

        int scaledIconSize = DpiHelper.Scale(56, factor);
        _iconPicture.IconSize = scaledIconSize;
        _iconPicture.Size = new Size(scaledIconSize + 12, scaledIconSize + 12);

        int totalContentHeight = _iconPicture.Height +
                                 DpiHelper.Scale(12, factor) +
                                 _titleLabel.Height +
                                 DpiHelper.Scale(6, factor) +
                                 _descriptionLabel.Height +
                                 (_showActionButton ? DpiHelper.Scale(16, factor) + _actionButton.Height : 0);

        int currentY = Math.Max(10, (Height - totalContentHeight) / 2);

        // Căn giữa icon
        _iconPicture.Location = new Point((Width - _iconPicture.Width) / 2, currentY);
        currentY += _iconPicture.Height + DpiHelper.Scale(12, factor);

        // Căn giữa tiêu đề (độ rộng thoải mái, tránh tràn chữ)
        _titleLabel.Height = DpiHelper.Scale(32, factor);
        _titleLabel.Width = Math.Min(Width - 20, DpiHelper.Scale(550, factor));
        _titleLabel.Location = new Point((Width - _titleLabel.Width) / 2, currentY);
        currentY += _titleLabel.Height + DpiHelper.Scale(6, factor);

        // Căn giữa mô tả
        _descriptionLabel.Height = DpiHelper.Scale(48, factor);
        _descriptionLabel.Width = Math.Min(Width - 30, DpiHelper.Scale(550, factor));
        _descriptionLabel.Location = new Point((Width - _descriptionLabel.Width) / 2, currentY);
        currentY += _descriptionLabel.Height + DpiHelper.Scale(16, factor);

        // Căn giữa nút bấm
        if (_showActionButton)
        {
            _actionButton.Location = new Point((Width - _actionButton.Width) / 2, currentY);
        }
    }
}
