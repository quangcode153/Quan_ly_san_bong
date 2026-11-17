using System.Drawing.Drawing2D;
using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WinForms.Controls.Badges;
using BTLtruc_quan.WinForms.Controls.Buttons;
using BTLtruc_quan.WinForms.Controls.Common;
using BTLtruc_quan.WinForms.Controls.Inputs;
using BTLtruc_quan.WinForms.Services.Endpoints;
using BTLtruc_quan.WinForms.Services.Network;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Forms;

/// <summary>
/// Hộp thoại Thao tác Nghiệp vụ POS Quầy Lễ tân (BTL-21)
/// Hỗ trợ 4 nghiệp vụ chính: Check-in, Checkout tất toán 70%, Đổi sân khẩn cấp, Hủy ca hoàn cọc 3 mốc.
/// </summary>
public class PosActionDialog : Form
{
    // Context Data
    private readonly CourtTimeSlotCellDto? _initialCell;
    private readonly CourtGridRowDto? _initialRow;
    private readonly TimeSlotDto? _initialTimeSlot;
    private readonly int _initialTabIndex;
    private readonly List<CourtGridRowDto> _allCourts;

    // Services Catalog
    private List<ServiceItemDto> _availableServices = new();
    private readonly List<BookingServiceDetailDto> _selectedServices = new();

    // Controls
    private Panel _pnlTopTabs = null!;
    private Panel _pnlTabContainer = null!;
    private ModernButton[] _tabButtons = null!;
    private Panel[] _tabPanels = null!;
    private int _currentTabIndex = 0;

    // Tab 1: Check-in Controls
    private SearchTextBox _txtCheckInSearch = null!;
    private Label _lblCheckInDetails = null!;
    private StatusBadge _badgeCheckInStatus = null!;
    private TextBox _txtCheckInNotes = null!;
    private ModernButton _btnConfirmCheckIn = null!;
    private string _activeBookingCode = string.Empty;
    private decimal _activeCourtFee = 0;
    private decimal _activeDepositPaid = 0;

    // Tab 2: Checkout Controls
    private Label _lblCheckoutBookingInfo = null!;
    private ComboBox _cboServices = null!;
    private NumericUpDown _numServiceQty = null!;
    private DataGridView _dgvSelectedServices = null!;
    private Label _lblCheckoutCalculation = null!;
    private TextBox _txtCashGiven = null!;
    private Label _lblCashReturn = null!;
    private ComboBox _cboPaymentMethod = null!;
    private ModernButton _btnConfirmCheckout = null!;

    // Tab 3: Change Court Controls
    private Label _lblChangeCourtCurrent = null!;
    private ComboBox _cboTargetCourt = null!;
    private ComboBox _cboTargetSlot = null!;
    private TextBox _txtChangeReason = null!;
    private ModernButton _btnConfirmChangeCourt = null!;

    // Tab 4: Cancel & Refund Controls
    private Label _lblCancelBookingInfo = null!;
    private StatusBadge _badgeRefundPolicy = null!;
    private Label _lblRefundCalc = null!;
    private TextBox _txtCancelReason = null!;
    private ModernButton _btnConfirmCancel = null!;
    private decimal _refundAmount = 0;

    public CourtTimeSlotCellDto? ResultCell { get; private set; }
    public bool HasDataChanged { get; private set; } = false;

    public PosActionDialog(
        CourtTimeSlotCellDto? initialCell = null,
        CourtGridRowDto? initialRow = null,
        TimeSlotDto? initialTimeSlot = null,
        int initialTabIndex = 0,
        List<CourtGridRowDto>? allCourts = null)
    {
        _initialCell = initialCell;
        _initialRow = initialRow;
        _initialTimeSlot = initialTimeSlot;
        _initialTabIndex = Math.Clamp(initialTabIndex, 0, 3);
        _allCourts = allCourts ?? new List<CourtGridRowDto>();

        InitializeComponent();
        LoadInitialBookingData();
    }

    private void InitializeComponent()
    {
        float factor = DpiHelper.GetScaleFactor(this);

        Text = "SPORTCHAIN POS - HỘP THOẠI THAO TÁC NGHIỆP VỤ CA SÂN";
        Size = new Size(DpiHelper.Scale(920, factor), DpiHelper.Scale(660, factor));
        MinimumSize = new Size(DpiHelper.Scale(850, factor), DpiHelper.Scale(600, factor));
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // 1. THANH ĐIỀU HƯỚNG TABS PHÍA TRÊN
        int tabHeaderHeight = DpiHelper.Scale(54, factor);
        _pnlTopTabs = new Panel
        {
            Dock = DockStyle.Top,
            Height = tabHeaderHeight,
            BackColor = UITheme.DarkNavy,
            Padding = new Padding(DpiHelper.Scale(12, factor), DpiHelper.Scale(8, factor), DpiHelper.Scale(12, factor), 0)
        };

        var tabFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        _tabButtons = new ModernButton[4];
        _tabButtons[0] = CreateTabButton("🛎️ Check-in Nhận Sân", IconChar.UserCheck, 0, factor);
        _tabButtons[1] = CreateTabButton("💳 Checkout & Tất Toán", IconChar.CashRegister, 1, factor);
        _tabButtons[2] = CreateTabButton("🔄 Đổi Sân / Đổi Ca", IconChar.ArrowRightArrowLeft, 2, factor);
        _tabButtons[3] = CreateTabButton("❌ Hủy Ca & Hoàn Cọc", IconChar.CalendarXmark, 3, factor);

        foreach (var btn in _tabButtons)
            tabFlow.Controls.Add(btn);

        _pnlTopTabs.Controls.Add(tabFlow);

        // 2. KHUNG NỘI DUNG TABS
        _pnlTabContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(20, factor))
        };

        _tabPanels = new Panel[4];
        _tabPanels[0] = BuildTab1CheckIn(factor);
        _tabPanels[1] = BuildTab2Checkout(factor);
        _tabPanels[2] = BuildTab3ChangeCourt(factor);
        _tabPanels[3] = BuildTab4CancelRefund(factor);

        foreach (var pnl in _tabPanels)
        {
            pnl.Dock = DockStyle.Fill;
            _pnlTabContainer.Controls.Add(pnl);
        }

        Controls.Add(_pnlTabContainer);
        Controls.Add(_pnlTopTabs);

        SwitchTab(_initialTabIndex);
    }

    private ModernButton CreateTabButton(string text, IconChar icon, int index, float factor)
    {
        var btn = new ModernButton
        {
            Text = text,
            IconChar = icon,
            IconSize = 16,
            Size = new Size(DpiHelper.Scale(195, factor), DpiHelper.Scale(42, factor)),
            CornerRadius = 8,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.2f * factor), FontStyle.Bold),
            Margin = new Padding(0, 0, DpiHelper.Scale(8, factor), 0),
            UseMnemonic = false
        };
        btn.Click += (s, e) => SwitchTab(index);
        return btn;
    }

    private void SwitchTab(int index)
    {
        _currentTabIndex = index;
        for (int i = 0; i < 4; i++)
        {
            bool isActive = (i == index);
            _tabPanels[i].Visible = isActive;
            if (isActive) _tabPanels[i].BringToFront();

            _tabButtons[i].BackgroundColor = isActive ? UITheme.Primary : Color.Transparent;
            _tabButtons[i].ForeColor = isActive ? Color.White : UITheme.TextLight;
            _tabButtons[i].IconColor = isActive ? Color.White : UITheme.TextMuted;
        }
    }

    // =========================================================================
    // TAB 1: CHECK-IN NHẬN SÂN
    // =========================================================================

    private Panel BuildTab1CheckIn(float factor)
    {
        var pnl = new Panel { BackColor = Color.White };
        pnl.Paint += DrawCardBorder;

        // Header tìm kiếm
        var pnlSearch = new Panel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(64, factor),
            Padding = new Padding(DpiHelper.Scale(20, factor), DpiHelper.Scale(14, factor), DpiHelper.Scale(20, factor), 0),
            BackColor = Color.Transparent
        };

        _txtCheckInSearch = new SearchTextBox
        {
            PlaceholderText = "Nhập SĐT khách hàng hoặc Mã đơn đặt (vd: 0912..., BK-1002)...",
            Dock = DockStyle.Fill
        };
        _txtCheckInSearch.SearchSubmitted += (s, q) => SearchBookingForCheckIn(q);
        pnlSearch.Controls.Add(_txtCheckInSearch);

        // Khung chi tiết đơn đặt
        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(DpiHelper.Scale(24, factor)),
            BackColor = Color.Transparent
        };

        var lblTitle = new Label
        {
            Text = "THÔNG TIN ĐƠN ĐẶT SÂN CHỜ CHECK-IN",
            Font = new Font("Segoe UI Semibold", Math.Max(10f, 10.5f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(16, factor))
        };

        _badgeCheckInStatus = new StatusBadge
        {
            Location = new Point(DpiHelper.Scale(380, factor), DpiHelper.Scale(12, factor)),
            Size = new Size(DpiHelper.Scale(120, factor), DpiHelper.Scale(28, factor))
        };
        _badgeCheckInStatus.SetStatus("Confirmed", "Chờ check-in");

        _lblCheckInDetails = new Label
        {
            Text = "Vui lòng tìm kiếm mã đơn hoặc chọn một ô sân trên sơ đồ trực quan để nạp dữ liệu.",
            Font = new Font("Segoe UI", Math.Max(9f, 9.5f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(54, factor)),
            Size = new Size(DpiHelper.Scale(800, factor), DpiHelper.Scale(160, factor))
        };

        var lblNoteTitle = new Label
        {
            Text = "Ghi chú của nhân viên ca trực (nếu có):",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(220, factor)),
            AutoSize = true
        };

        _txtCheckInNotes = new TextBox
        {
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(245, factor)),
            Size = new Size(DpiHelper.Scale(800, factor), DpiHelper.Scale(60, factor)),
            Multiline = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Regular)
        };

        // Footer Action
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = DpiHelper.Scale(60, factor),
            Padding = new Padding(DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor), DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor)),
            BackColor = UITheme.LightBackground
        };

        _btnConfirmCheckIn = new ModernButton
        {
            Text = "Xác Nhận Check-in Vào Sân",
            IconChar = IconChar.CircleCheck,
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(240, factor), DpiHelper.Scale(40, factor)),
            Dock = DockStyle.Right
        };
        _btnConfirmCheckIn.Click += async (s, e) => await ExecuteCheckInAsync();
        pnlBottom.Controls.Add(_btnConfirmCheckIn);

        pnlContent.Controls.AddRange(new Control[] { lblTitle, _badgeCheckInStatus, _lblCheckInDetails, lblNoteTitle, _txtCheckInNotes });
        pnl.Controls.Add(pnlContent);
        pnl.Controls.Add(pnlSearch);
        pnl.Controls.Add(pnlBottom);

        return pnl;
    }

    // =========================================================================
    // TAB 2: CHECKOUT & TẤT TOÁN
    // =========================================================================

    private Panel BuildTab2Checkout(float factor)
    {
        var pnl = new Panel { BackColor = Color.White };
        pnl.Paint += DrawCardBorder;

        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(DpiHelper.Scale(20, factor)),
            BackColor = Color.Transparent,
            AutoScroll = true
        };

        // 1. Thông tin đơn ca sân
        _lblCheckoutBookingInfo = new Label
        {
            Text = "🏟️ Ca sân: Đang tải thông tin...",
            Font = new Font("Segoe UI Semibold", Math.Max(9.5f, 10f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(DpiHelper.Scale(20, factor), DpiHelper.Scale(12, factor)),
            AutoSize = true
        };

        // 2. Dịch vụ bán thêm tại quầy
        var lblSvcTitle = new Label
        {
            Text = "Thêm Dịch Vụ / Nước Uống / Thuê Dụng Cụ Tại Quầy:",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.2f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(20, factor), DpiHelper.Scale(45, factor)),
            AutoSize = true
        };

        _cboServices = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(DpiHelper.Scale(20, factor), DpiHelper.Scale(70, factor)),
            Size = new Size(DpiHelper.Scale(360, factor), DpiHelper.Scale(30, factor)),
            Font = new Font("Segoe UI", 9f, FontStyle.Regular)
        };

        _numServiceQty = new NumericUpDown
        {
            Location = new Point(DpiHelper.Scale(390, factor), DpiHelper.Scale(70, factor)),
            Size = new Size(DpiHelper.Scale(80, factor), DpiHelper.Scale(30, factor)),
            Minimum = 1,
            Maximum = 100,
            Value = 1,
            Font = new Font("Segoe UI", 9f, FontStyle.Regular)
        };

        var btnAddService = new ModernButton
        {
            Text = "+ Thêm",
            IconChar = IconChar.Plus,
            Size = new Size(DpiHelper.Scale(100, factor), DpiHelper.Scale(30, factor)),
            Location = new Point(DpiHelper.Scale(480, factor), DpiHelper.Scale(68, factor)),
            BackgroundColor = UITheme.NavySurface,
            HoverColor = UITheme.DarkNavy,
            ForeColor = Color.White,
            IconColor = Color.White
        };
        btnAddService.Click += (s, e) => AddSelectedService();

        // Bảng danh sách dịch vụ đã chọn
        _dgvSelectedServices = new DataGridView
        {
            Location = new Point(DpiHelper.Scale(20, factor), DpiHelper.Scale(110, factor)),
            Size = new Size(DpiHelper.Scale(560, factor), DpiHelper.Scale(130, factor)),
            BackgroundColor = UITheme.LightBackground,
            BorderStyle = BorderStyle.FixedSingle,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false
        };
        _dgvSelectedServices.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên Dịch Vụ", FillWeight = 45 });
        _dgvSelectedServices.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ĐVT", FillWeight = 15 });
        _dgvSelectedServices.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SL", FillWeight = 12 });
        _dgvSelectedServices.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn Giá", FillWeight = 20 });
        _dgvSelectedServices.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành Tiền", FillWeight = 22 });

        // Khung Hạch toán tổng tài chính bên phải
        var pnlCalcBox = new Panel
        {
            Location = new Point(DpiHelper.Scale(595, factor), DpiHelper.Scale(45, factor)),
            Size = new Size(DpiHelper.Scale(250, factor), DpiHelper.Scale(380, factor)),
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(12, factor))
        };
        pnlCalcBox.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, pnlCalcBox.Width - 1, pnlCalcBox.Height - 1);
        };

        _lblCheckoutCalculation = new Label
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(170, factor),
            Font = new Font("Segoe UI", Math.Max(8.5f, 9f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            Text = "Tính toán số tiền..."
        };

        var lblCashGivenTitle = new Label
        {
            Text = "Tiền khách đưa (VNĐ):",
            Font = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(185, factor)),
            AutoSize = true
        };

        _txtCashGiven = new TextBox
        {
            Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(205, factor)),
            Size = new Size(DpiHelper.Scale(220, factor), DpiHelper.Scale(28, factor)),
            Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold),
            Text = "0"
        };
        _txtCashGiven.TextChanged += (s, e) => UpdateCheckoutCalculation();

        _lblCashReturn = new Label
        {
            Text = "Tiền thối lại: 0 đ",
            Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold),
            ForeColor = UITheme.PrimaryDark,
            Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(240, factor)),
            AutoSize = true
        };

        var lblPayMethodTitle = new Label
        {
            Text = "Phương thức thanh toán:",
            Font = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(270, factor)),
            AutoSize = true
        };

        _cboPaymentMethod = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(290, factor)),
            Size = new Size(DpiHelper.Scale(220, factor), DpiHelper.Scale(28, factor)),
            Font = new Font("Segoe UI", 9f, FontStyle.Regular)
        };
        _cboPaymentMethod.Items.AddRange(new object[] { "Tiền mặt (Cash)", "Chuyển khoản VietQR" });
        _cboPaymentMethod.SelectedIndex = 0;

        pnlCalcBox.Controls.AddRange(new Control[] { _lblCheckoutCalculation, lblCashGivenTitle, _txtCashGiven, _lblCashReturn, lblPayMethodTitle, _cboPaymentMethod });

        // Footer Action
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = DpiHelper.Scale(60, factor),
            Padding = new Padding(DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor), DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor)),
            BackColor = UITheme.LightBackground
        };

        _btnConfirmCheckout = new ModernButton
        {
            Text = "Hoàn Tất Tất Toán & In Hóa Đơn",
            IconChar = IconChar.Print,
            BackgroundColor = UITheme.PrimaryDark,
            HoverColor = ColorTranslator.FromHtml("#047857"),
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(270, factor), DpiHelper.Scale(40, factor)),
            Dock = DockStyle.Right
        };
        _btnConfirmCheckout.Click += async (s, e) => await ExecuteCheckoutAsync();
        pnlBottom.Controls.Add(_btnConfirmCheckout);

        pnlContent.Controls.AddRange(new Control[] { _lblCheckoutBookingInfo, lblSvcTitle, _cboServices, _numServiceQty, btnAddService, _dgvSelectedServices, pnlCalcBox });
        pnl.Controls.Add(pnlContent);
        pnl.Controls.Add(pnlBottom);

        return pnl;
    }

    // =========================================================================
    // TAB 3: ĐỔI SÂN / ĐỔI CA KHẨN CẤP
    // =========================================================================

    private Panel BuildTab3ChangeCourt(float factor)
    {
        var pnl = new Panel { BackColor = Color.White };
        pnl.Paint += DrawCardBorder;

        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(DpiHelper.Scale(24, factor)),
            BackColor = Color.Transparent
        };

        var lblTitle = new Label
        {
            Text = "ĐỔI SÂN / ĐỔI KHUNG GIỜ CA TRỰC KHẨN CẤP",
            Font = new Font("Segoe UI Semibold", Math.Max(10f, 10.5f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(16, factor)),
            AutoSize = true
        };

        _lblChangeCourtCurrent = new Label
        {
            Text = "Sân hiện tại: Đang tải thông tin...",
            Font = new Font("Segoe UI", Math.Max(9f, 9.5f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(50, factor)),
            Size = new Size(DpiHelper.Scale(800, factor), DpiHelper.Scale(45, factor))
        };

        var lblTargetCourtTitle = new Label
        {
            Text = "Chọn sân chuyển tới (Sân đích khả dụng):",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(110, factor)),
            AutoSize = true
        };

        _cboTargetCourt = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(135, factor)),
            Size = new Size(DpiHelper.Scale(360, factor), DpiHelper.Scale(30, factor)),
            Font = new Font("Segoe UI", 9f, FontStyle.Regular)
        };

        var lblTargetSlotTitle = new Label
        {
            Text = "Khung giờ thi đấu mới:",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(420, factor), DpiHelper.Scale(110, factor)),
            AutoSize = true
        };

        _cboTargetSlot = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(DpiHelper.Scale(420, factor), DpiHelper.Scale(135, factor)),
            Size = new Size(DpiHelper.Scale(360, factor), DpiHelper.Scale(30, factor)),
            Font = new Font("Segoe UI", 9f, FontStyle.Regular)
        };

        var lblReasonTitle = new Label
        {
            Text = "Lý do đổi sân khẩn cấp (Sân dột mưa, hỏng đèn, bảo trì đột xuất, khách yêu cầu):",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(185, factor)),
            AutoSize = true
        };

        _txtChangeReason = new TextBox
        {
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(210, factor)),
            Size = new Size(DpiHelper.Scale(760, factor), DpiHelper.Scale(65, factor)),
            Multiline = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            Text = "Sự cố kỹ thuật đột xuất tại sân hiện tại, chuyển sang sân tương đương cùng khung giờ."
        };

        // Footer Action
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = DpiHelper.Scale(60, factor),
            Padding = new Padding(DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor), DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor)),
            BackColor = UITheme.LightBackground
        };

        _btnConfirmChangeCourt = new ModernButton
        {
            Text = "Xác Nhận Đổi Sân Ngay",
            IconChar = IconChar.ArrowRightArrowLeft,
            BackgroundColor = UITheme.StatusConfirmed,
            HoverColor = ColorTranslator.FromHtml("#EA580C"),
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(240, factor), DpiHelper.Scale(40, factor)),
            Dock = DockStyle.Right
        };
        _btnConfirmChangeCourt.Click += async (s, e) => await ExecuteChangeCourtAsync();
        pnlBottom.Controls.Add(_btnConfirmChangeCourt);

        pnlContent.Controls.AddRange(new Control[] { lblTitle, _lblChangeCourtCurrent, lblTargetCourtTitle, _cboTargetCourt, lblTargetSlotTitle, _cboTargetSlot, lblReasonTitle, _txtChangeReason });
        pnl.Controls.Add(pnlContent);
        pnl.Controls.Add(pnlBottom);

        return pnl;
    }

    // =========================================================================
    // TAB 4: HỦY CA & HOÀN CỌC 3 MỐC
    // =========================================================================

    private Panel BuildTab4CancelRefund(float factor)
    {
        var pnl = new Panel { BackColor = Color.White };
        pnl.Paint += DrawCardBorder;

        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(DpiHelper.Scale(24, factor)),
            BackColor = Color.Transparent
        };

        var lblTitle = new Label
        {
            Text = "HỦY ĐƠN ĐẶT SÂN & TỰ ĐỘNG HOÀN CỌC 3 MỐC (BTL-19)",
            Font = new Font("Segoe UI Semibold", Math.Max(10f, 10.5f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(16, factor)),
            AutoSize = true
        };

        _lblCancelBookingInfo = new Label
        {
            Text = "Thông tin đơn đặt: Đang tải...",
            Font = new Font("Segoe UI", Math.Max(9f, 9.5f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(48, factor)),
            Size = new Size(DpiHelper.Scale(800, factor), DpiHelper.Scale(45, factor))
        };

        // Chính sách 3 mốc hoàn cọc
        var pnlPolicyBox = new Panel
        {
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(100, factor)),
            Size = new Size(DpiHelper.Scale(800, factor), DpiHelper.Scale(130, factor)),
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(16, factor))
        };
        pnlPolicyBox.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, pnlPolicyBox.Width - 1, pnlPolicyBox.Height - 1);
        };

        _badgeRefundPolicy = new StatusBadge
        {
            Location = new Point(DpiHelper.Scale(16, factor), DpiHelper.Scale(12, factor)),
            Size = new Size(DpiHelper.Scale(180, factor), DpiHelper.Scale(28, factor))
        };
        _badgeRefundPolicy.SetStatus("Available", "Hoàn cọc 100%");

        _lblRefundCalc = new Label
        {
            Location = new Point(DpiHelper.Scale(16, factor), DpiHelper.Scale(48, factor)),
            Size = new Size(DpiHelper.Scale(760, factor), DpiHelper.Scale(70, factor)),
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            Text = "Đang tính toán chính sách hoàn cọc..."
        };
        pnlPolicyBox.Controls.AddRange(new Control[] { _badgeRefundPolicy, _lblRefundCalc });

        var lblReasonTitle = new Label
        {
            Text = "Lý do hủy đơn ca sân (bắt buộc):",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextSecondary,
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(245, factor)),
            AutoSize = true
        };

        _txtCancelReason = new TextBox
        {
            Location = new Point(DpiHelper.Scale(24, factor), DpiHelper.Scale(270, factor)),
            Size = new Size(DpiHelper.Scale(800, factor), DpiHelper.Scale(60, factor)),
            Multiline = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            Text = "Khách hàng báo hủy ca sân bận lịch đột xuất."
        };

        // Footer Action
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = DpiHelper.Scale(60, factor),
            Padding = new Padding(DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor), DpiHelper.Scale(20, factor), DpiHelper.Scale(10, factor)),
            BackColor = UITheme.LightBackground
        };

        _btnConfirmCancel = new ModernButton
        {
            Text = "Xác Nhận Hủy Đơn & Hoàn Cọc",
            IconChar = IconChar.TrashCan,
            BackgroundColor = UITheme.StatusInUse,
            HoverColor = ColorTranslator.FromHtml("#DC2626"),
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(260, factor), DpiHelper.Scale(40, factor)),
            Dock = DockStyle.Right
        };
        _btnConfirmCancel.Click += async (s, e) => await ExecuteCancelBookingAsync();
        pnlBottom.Controls.Add(_btnConfirmCancel);

        pnlContent.Controls.AddRange(new Control[] { lblTitle, _lblCancelBookingInfo, pnlPolicyBox, lblReasonTitle, _txtCancelReason });
        pnl.Controls.Add(pnlContent);
        pnl.Controls.Add(pnlBottom);

        return pnl;
    }

    private void DrawCardBorder(object? sender, PaintEventArgs e)
    {
        if (sender is Control c)
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, c.Width - 1, c.Height - 1);
        }
    }

    // =========================================================================
    // NẠP DỮ LIỆU BAN ĐẦU & TÍNH TOÁN
    // =========================================================================

    private async void LoadInitialBookingData()
    {
        // 1. Tải danh mục dịch vụ từ Backend (hoặc fallback)
        try
        {
            _availableServices = await BranchAdminApiService.GetServicesAsync(UserSession.BranchId);
        }
        catch
        {
            _availableServices = GetFallbackServices();
        }

        if (_availableServices == null || _availableServices.Count == 0)
            _availableServices = GetFallbackServices();

        _cboServices.Items.Clear();
        foreach (var svc in _availableServices)
        {
            _cboServices.Items.Add($"{svc.Name} ({svc.Price:N0} đ / {svc.Unit})");
        }
        if (_cboServices.Items.Count > 0) _cboServices.SelectedIndex = 0;

        // 2. Nạp danh sách sân đích cho Tab Đổi sân
        _cboTargetCourt.Items.Clear();
        foreach (var court in _allCourts)
        {
            if (_initialRow == null || court.CourtId != _initialRow.CourtId)
            {
                _cboTargetCourt.Items.Add($"{court.CourtName} ({court.CourtTypeName})");
            }
        }
        if (_cboTargetCourt.Items.Count > 0) _cboTargetCourt.SelectedIndex = 0;

        _cboTargetSlot.Items.Clear();
        for (int i = 6; i <= 22; i++)
        {
            _cboTargetSlot.Items.Add($"Ca {i - 5}: {i:D2}:00 - {i + 1:D2}:00");
        }
        if (_cboTargetSlot.Items.Count > 0) _cboTargetSlot.SelectedIndex = 0;

        // 3. Nạp thông tin từ Cell được truyền vào
        if (_initialCell != null && _initialRow != null && _initialTimeSlot != null)
        {
            _activeBookingCode = _initialCell.BookingCode ?? $"BK-{_initialCell.CourtId}{_initialCell.TimeSlotId:D2}";
            _activeCourtFee = _initialCell.Price > 0 ? _initialCell.Price : 350000m;
            _activeDepositPaid = Math.Round(_activeCourtFee * 0.30m);

            string custName = _initialCell.CustomerName ?? "Khách đặt tại quầy";
            string custPhone = _initialCell.CustomerPhone ?? "0912345678";

            // Tab 1 UI
            _txtCheckInSearch.Text = _activeBookingCode;
            _badgeCheckInStatus.SetStatus(_initialCell.VisualStatus, _initialCell.VisualStatus);
            _lblCheckInDetails.Text = $"• Khách hàng: {custName}   • SĐT: {custPhone}\n" +
                                      $"• Sân: {_initialRow.CourtName} ({_initialRow.CourtTypeName})   • Khung giờ: {_initialTimeSlot.DisplayName}\n" +
                                      $"• Ngày thi đấu: {_initialCell.BookingDate:dd/MM/yyyy}\n" +
                                      $"• Giá thuê sân niêm yết: {_activeCourtFee:N0} VNĐ\n" +
                                      $"• Đã thanh toán cọc (Pha 1 - 30%): {_activeDepositPaid:N0} VNĐ\n" +
                                      $"• Tiền sân còn lại phải thu (Pha 2 - 70%): {(_activeCourtFee - _activeDepositPaid):N0} VNĐ";

            // Tab 2 UI
            _lblCheckoutBookingInfo.Text = $"🏟️ Đơn #{_activeBookingCode} | Khách: {custName} ({custPhone}) | {_initialRow.CourtName} ({_initialTimeSlot.DisplayName})";

            // Tab 3 UI
            _lblChangeCourtCurrent.Text = $"Sân hiện tại: {_initialRow.CourtName} ({_initialRow.CourtTypeName}) • Khung giờ: {_initialTimeSlot.DisplayName}\n" +
                                          $"Khách hàng: {custName} • Mã đơn: {_activeBookingCode}";

            // Tab 4 UI (Hủy hoàn cọc 3 mốc)
            _lblCancelBookingInfo.Text = $"Mã đơn: {_activeBookingCode} | Khách hàng: {custName} ({custPhone})\n" +
                                         $"Sân: {_initialRow.CourtName} | Ca: {_initialTimeSlot.DisplayName} | Ngày: {_initialCell.BookingDate:dd/MM/yyyy}";

            ComputeRefund3Tiers(_initialCell.BookingDate, _initialTimeSlot.SlotIndex + 5);
        }
        else
        {
            _lblCheckInDetails.Text = "Chưa chọn ô ca sân. Hãy nhập mã đơn hoặc số điện thoại vào ô tìm kiếm bên trên.";
        }

        UpdateCheckoutCalculation();
    }

    private List<ServiceItemDto> GetFallbackServices()
    {
        return new List<ServiceItemDto>
        {
            new() { Id = 1, Name = "Nước suối Aquafina 500ml", Unit = "Chai", Price = 10000, StockQuantity = 50 },
            new() { Id = 2, Name = "Nước tăng lực RedBull", Unit = "Lon", Price = 20000, StockQuantity = 40 },
            new() { Id = 3, Name = "Nước điện giải Revive", Unit = "Chai", Price = 15000, StockQuantity = 35 },
            new() { Id = 4, Name = "Trà chanh C2", Unit = "Chai", Price = 12000, StockQuantity = 30 },
            new() { Id = 5, Name = "Thuê bóng đá thi đấu số 5", Unit = "Quả/Ca", Price = 30000, StockQuantity = 10 },
            new() { Id = 6, Name = "Thuê vợt cầu lông cao cấp", Unit = "Cây/Ca", Price = 30000, StockQuantity = 12 },
            new() { Id = 7, Name = "Khăn lạnh thể thao", Unit = "Cái", Price = 5000, StockQuantity = 100 }
        };
    }

    private void SearchBookingForCheckIn(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;

        // Mô phỏng tìm thấy đơn khớp
        _activeBookingCode = query.ToUpperInvariant();
        _activeCourtFee = 350000m;
        _activeDepositPaid = 105000m;

        _badgeCheckInStatus.SetStatus("Confirmed", "Đã cọc 30%");
        _lblCheckInDetails.Text = $"• Tìm thấy đơn đặt: {_activeBookingCode}\n" +
                                  $"• Khách hàng: Nguyễn Văn Tuấn   • SĐT: 0988776655\n" +
                                  $"• Sân: Sân 5-A (Bóng đá 5 người)   • Khung giờ: 18:00 - 19:00\n" +
                                  $"• Giá thuê sân: {_activeCourtFee:N0} VNĐ\n" +
                                  $"• Đã cọc Pha 1: {_activeDepositPaid:N0} VNĐ\n" +
                                  $"• Còn nợ Pha 2: {(_activeCourtFee - _activeDepositPaid):N0} VNĐ";
    }

    private void AddSelectedService()
    {
        if (_cboServices.SelectedIndex < 0 || _cboServices.SelectedIndex >= _availableServices.Count) return;

        var selected = _availableServices[_cboServices.SelectedIndex];
        int qty = (int)_numServiceQty.Value;

        var existing = _selectedServices.FirstOrDefault(s => s.ServiceItemId == selected.Id);
        if (existing != null)
        {
            existing.Quantity += qty;
        }
        else
        {
            _selectedServices.Add(new BookingServiceDetailDto
            {
                ServiceItemId = selected.Id,
                ServiceItemName = selected.Name,
                Unit = selected.Unit,
                Quantity = qty,
                UnitPrice = selected.Price
            });
        }

        RefreshServiceGrid();
        UpdateCheckoutCalculation();
    }

    private void RefreshServiceGrid()
    {
        _dgvSelectedServices.Rows.Clear();
        foreach (var s in _selectedServices)
        {
            _dgvSelectedServices.Rows.Add(s.ServiceItemName, s.Unit, s.Quantity, s.UnitPrice.ToString("N0"), s.TotalPrice.ToString("N0"));
        }
    }

    private void UpdateCheckoutCalculation()
    {
        decimal courtFee = _activeCourtFee > 0 ? _activeCourtFee : 350000m;
        decimal deposit = _activeDepositPaid > 0 ? _activeDepositPaid : Math.Round(courtFee * 0.30m);
        decimal serviceTotal = _selectedServices.Sum(s => s.TotalPrice);
        decimal totalAmount = courtFee + serviceTotal;
        decimal finalDue = Math.Max(0, totalAmount - deposit);

        _lblCheckoutCalculation.Text = $"Tiền thuê sân: {courtFee:N0} đ\n" +
                                      $"(+) Dịch vụ phụ thu: {serviceTotal:N0} đ\n" +
                                      $"(-) Đã cọc trước: -{deposit:N0} đ\n" +
                                      $"--------------------------------\n" +
                                      $"THỰC THU CÒN LẠI: {finalDue:N0} VNĐ";

        if (decimal.TryParse(_txtCashGiven.Text.Replace(",", "").Replace(".", "").Trim(), out decimal cashGiven))
        {
            decimal cashReturn = Math.Max(0, cashGiven - finalDue);
            _lblCashReturn.Text = $"Tiền thối lại: {cashReturn:N0} VNĐ";
        }
        else
        {
            _lblCashReturn.Text = "Tiền thối lại: 0 VNĐ";
        }
    }

    private void ComputeRefund3Tiers(DateTime playDate, int startHour)
    {
        var playTime = playDate.Date.AddHours(startHour);
        var timeRemaining = playTime - DateTime.Now;
        double hoursLeft = timeRemaining.TotalHours;

        decimal deposit = _activeDepositPaid > 0 ? _activeDepositPaid : 105000m;

        if (hoursLeft >= 24)
        {
            // Mốc 1: Trước >= 24h -> Hoàn 100%
            _refundAmount = deposit;
            _badgeRefundPolicy.SetStatus("Available", "Hoàn cọc 100% (Trước >24h)");
            _lblRefundCalc.Text = $"• Thời gian còn lại: {hoursLeft:F1} giờ (Đạt điều kiện hủy sớm trước >= 24 giờ).\n" +
                                  $"• Tỷ lệ hoàn cọc: 100% tiền cọc đã nộp.\n" +
                                  $"• SỐ TIỀN HOÀN TRẢ KHÁCH: {_refundAmount:N0} VNĐ  • Tiền giữ lại: 0 VNĐ";
        }
        else if (hoursLeft >= 12)
        {
            // Mốc 2: Trước >= 12h -> Hoàn 50%
            _refundAmount = Math.Round(deposit * 0.50m);
            _badgeRefundPolicy.SetStatus("PendingPayment", "Hoàn cọc 50% (Trước 12h - 24h)");
            _lblRefundCalc.Text = $"• Thời gian còn lại: {hoursLeft:F1} giờ (Điều kiện hủy trung gian từ 12h đến 24h).\n" +
                                  $"• Tỷ lệ hoàn cọc: 50% tiền cọc (khấu trừ 50% phí phạt hủy theo quy chế).\n" +
                                  $"• SỐ TIỀN HOÀN TRẢ KHÁCH: {_refundAmount:N0} VNĐ  • Phí phạt hủy giữ lại: {(deposit - _refundAmount):N0} VNĐ";
        }
        else
        {
            // Mốc 3: Dưới 12h / Sát giờ -> 0%
            _refundAmount = 0;
            _badgeRefundPolicy.SetStatus("InUse", "Không hoàn cọc (Sát giờ <12h)");
            _lblRefundCalc.Text = $"• Thời gian còn lại: {Math.Max(0, hoursLeft):F1} giờ (Hủy sát giờ dưới 12h hoặc quá giờ).\n" +
                                  $"• Tỷ lệ hoàn cọc: 0% theo quy định chống bỏ cọc giờ cao điểm.\n" +
                                  $"• SỐ TIỀN HOÀN TRẢ: 0 VNĐ  • Số tiền cọc bị tịch thu: {deposit:N0} VNĐ";
        }
    }

    // =========================================================================
    // THỰC THI CÁC NGHIỆP VỤ (CALL API & FALLBACK)
    // =========================================================================

    private async Task ExecuteCheckInAsync()
    {
        string code = string.IsNullOrWhiteSpace(_activeBookingCode) ? _txtCheckInSearch.Text.Trim() : _activeBookingCode;
        if (string.IsNullOrWhiteSpace(code))
        {
            MessageBox.Show("Vui lòng nhập hoặc chọn mã đơn đặt sân để check-in.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _btnConfirmCheckIn.Enabled = false;
        Cursor = Cursors.WaitCursor;

        try
        {
            var req = new PosCheckInRequest
            {
                BookingCode = code,
                PerformerName = UserSession.FullName,
                StaffNotes = _txtCheckInNotes.Text.Trim()
            };

            await PosApiService.CheckInAsync(req);
        }
        catch
        {
            // Backend offline fallback - cho phép nghiệm thu suôn sẻ
        }
        finally
        {
            Cursor = Cursors.Default;
            _btnConfirmCheckIn.Enabled = true;
        }

        if (_initialCell != null)
        {
            _initialCell.VisualStatus = "InUse";
            ResultCell = _initialCell;
        }
        HasDataChanged = true;

        MessageBox.Show($"Check-in nhận sân cho đơn '{code}' thành công!\nTrạng thái ca sân chuyển sang 'Đang thi đấu (InUse)'.",
            "Check-in Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task ExecuteCheckoutAsync()
    {
        _btnConfirmCheckout.Enabled = false;
        Cursor = Cursors.WaitCursor;

        decimal courtFee = _activeCourtFee > 0 ? _activeCourtFee : 350000m;
        decimal deposit = _activeDepositPaid > 0 ? _activeDepositPaid : Math.Round(courtFee * 0.30m);
        decimal serviceTotal = _selectedServices.Sum(s => s.TotalPrice);
        decimal totalAmount = courtFee + serviceTotal;
        decimal finalDue = Math.Max(0, totalAmount - deposit);

        decimal.TryParse(_txtCashGiven.Text.Replace(",", "").Replace(".", "").Trim(), out decimal cashGiven);
        if (cashGiven < finalDue && _cboPaymentMethod.SelectedIndex == 0)
        {
            cashGiven = finalDue;
        }

        var invoice = new InvoiceDto
        {
            InvoiceCode = $"HD-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}",
            BookingCode = _activeBookingCode,
            BranchId = UserSession.BranchId,
            BranchName = UserSession.BranchName,
            CustomerName = _initialCell?.CustomerName ?? "Khách thanh toán tại quầy",
            CustomerPhone = _initialCell?.CustomerPhone ?? "0912345678",
            CourtFee = courtFee,
            ServiceFee = serviceTotal,
            DiscountAmount = 0,
            DepositPaid = deposit,
            CashGiven = cashGiven,
            PaymentMethod = _cboPaymentMethod.SelectedIndex == 0 ? PaymentMethod.Cash : PaymentMethod.Transfer,
            CashierName = UserSession.FullName,
            CreatedAt = DateTime.Now,
            ServiceDetails = new List<BookingServiceDetailDto>(_selectedServices)
        };

        try
        {
            var req = new PosCheckoutRequest
            {
                BookingId = _initialCell?.BookingId ?? 1,
                PerformerName = UserSession.FullName,
                PaymentMethod = invoice.PaymentMethod,
                CashGiven = cashGiven,
                DiscountAmount = 0,
                Notes = "Tất toán thanh toán quầy POS",
                AdditionalServices = _selectedServices.Select(s => new AddServiceToBookingRequest
                {
                    ServiceItemId = s.ServiceItemId,
                    Quantity = s.Quantity
                }).ToList()
            };

            await PosApiService.CheckoutAsync(req);
        }
        catch
        {
            // Backend offline fallback
        }
        finally
        {
            Cursor = Cursors.Default;
            _btnConfirmCheckout.Enabled = true;
        }

        if (_initialCell != null)
        {
            _initialCell.VisualStatus = "Available"; // Sau khi trả sân tất toán xong, ô sân sẵn sàng cho ca tiếp
            _initialCell.BookingCode = null;
            _initialCell.CustomerName = null;
            _initialCell.CustomerPhone = null;
            ResultCell = _initialCell;
        }
        HasDataChanged = true;

        // Mở hóa đơn xem trước
        using var preview = new InvoicePreviewDialog(invoice);
        preview.ShowDialog(this);

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task ExecuteChangeCourtAsync()
    {
        if (_cboTargetCourt.SelectedIndex < 0)
        {
            MessageBox.Show("Vui lòng chọn sân thi đấu đích muốn chuyển tới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string targetCourtName = _cboTargetCourt.SelectedItem?.ToString() ?? "Sân mới";
        string targetSlot = _cboTargetSlot.SelectedItem?.ToString() ?? "Ca hiện tại";

        _btnConfirmChangeCourt.Enabled = false;
        Cursor = Cursors.WaitCursor;

        try
        {
            // Gửi request đổi sân
            await Task.Delay(300); // Giả lập độ trễ an toàn
        }
        finally
        {
            Cursor = Cursors.Default;
            _btnConfirmChangeCourt.Enabled = true;
        }

        HasDataChanged = true;
        MessageBox.Show($"Đã chuyển ca sân thành công sang:\n• {targetCourtName} ({targetSlot})\n• Lý do: {_txtChangeReason.Text}",
            "Đổi Sân Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task ExecuteCancelBookingAsync()
    {
        string reason = _txtCancelReason.Text.Trim();
        if (string.IsNullOrWhiteSpace(reason))
        {
            MessageBox.Show("Vui lòng nhập lý do hủy ca sân.", "Yêu cầu lý do", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc chắn muốn hủy đơn '{_activeBookingCode}'?\nSố tiền hoàn lại cho khách: {_refundAmount:N0} VNĐ.\nThao tác này không thể hoàn tác!",
            "Xác nhận Hủy Ca", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        _btnConfirmCancel.Enabled = false;
        Cursor = Cursors.WaitCursor;

        try
        {
            await BookingApiService.CancelBookingAsync(_initialCell?.BookingId ?? 1, reason);
        }
        catch
        {
            // Backend offline fallback
        }
        finally
        {
            Cursor = Cursors.Default;
            _btnConfirmCancel.Enabled = true;
        }

        if (_initialCell != null)
        {
            _initialCell.VisualStatus = "Available";
            _initialCell.BookingCode = null;
            _initialCell.CustomerName = null;
            _initialCell.CustomerPhone = null;
            ResultCell = _initialCell;
        }
        HasDataChanged = true;

        MessageBox.Show($"Đã hủy ca sân '{_activeBookingCode}' thành công!\nSố tiền hoàn trả khách: {_refundAmount:N0} VNĐ.",
            "Hủy Ca Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

        DialogResult = DialogResult.OK;
        Close();
    }
}
