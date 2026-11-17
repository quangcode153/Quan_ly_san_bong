using System.Drawing.Drawing2D;
using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WinForms.Controls.Badges;
using BTLtruc_quan.WinForms.Controls.Buttons;
using BTLtruc_quan.WinForms.Controls.Cards;
using BTLtruc_quan.WinForms.Controls.Common;
using BTLtruc_quan.WinForms.Controls.Grids;
using BTLtruc_quan.WinForms.Controls.Inputs;
using BTLtruc_quan.WinForms.Services.Endpoints;
using BTLtruc_quan.WinForms.Services.Network;
using BTLtruc_quan.WinForms.Services.Realtime;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Forms;

public partial class MainForm : Form
{
    // =========================================================================
    // FIELDS & CONTROLS
    // =========================================================================

    // Layout Containers
    private Panel _pnlSidebar = null!;
    private Panel _pnlMainWrapper = null!;
    private Panel _pnlHeader = null!;
    private Panel _pnlContent = null!;

    // Sidebar Items
    private Panel _pnlBrand = null!;
    private ModernButton _btnNavDashboard = null!;
    private ModernButton _btnNavSchedule = null!;
    private ModernButton _btnNavBookings = null!;
    private ModernButton _btnNavSettings = null!;
    private Label _lblSidebarFooter = null!;

    // Header Items
    private Label _lblHeaderTitle = null!;
    private Label _lblHeaderSubtitle = null!;
    private ModernButton _btnRefresh = null!;
    private Label _lblUserShift = null!;
    private StatusBadge _badgeShiftStatus = null!;

    // Content Pages
    private Panel _pnlDashboardView = null!;
    private Panel _pnlScheduleView = null!;
    private Panel _pnlBookingsView = null!;
    private Panel _pnlSettingsView = null!;

    // Schedule / Matrix Controls
    private CourtMatrixGrid _courtMatrixGrid = null!;
    private DateTimePicker _dtpDate = null!;
    private ComboBox _cboCourtTypeFilter = null!;
    private SearchTextBox _txtSearchCourt = null!;
    private Label _lblMatrixStatusInfo = null!;
    private CourtGridResponseDto _fullMatrixData = null!;

    // POS & Bookings Management Controls (BTL-21)
    private string _quickStatusFilter = "All";
    private DateTimePicker _dtpBookingsDate = null!;
    private ComboBox _cboBookingStatusFilter = null!;
    private SearchTextBox _txtSearchBookings = null!;
    private DataGridView _dgvBookings = null!;
    private EmptyStateControl _emptyBookingsState = null!;
    private Label _lblBookingsSummary = null!;
    private List<BookingDto> _allBookingsData = new();

    // Network & Realtime Services
    private CourtRealtimeClient? _courtRealtimeClient;

    // Current State
    private int _currentNavigationIndex = 0; // 0: Dashboard, 1: Schedule, 2: Bookings, 3: Settings

    // =========================================================================
    // CONSTRUCTOR & INITIALIZATION
    // =========================================================================

    public MainForm()
    {
        InitializeComponent();
        DoubleBuffered = true;

        // Chỉ kích hoạt DevMode nếu chưa có phiên đăng nhập hợp lệ
        if (!UserSession.IsLoggedIn)
        {
            UserSession.EnableDevMode();
        }
        UserSession.OnSessionExpired += HandleSessionExpired;

        BuildMainLayout();
        _fullMatrixData = GenerateMockData(DateTime.Today);
        ApplyCourtMatrixFilter();
        NavigateTo(0); // Mặc định mở Dashboard
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadMatrixDataAsync();
        await InitializeRealtimeAsync();
    }

    private void HandleSessionExpired()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(HandleSessionExpired));
            return;
        }

        MessageBox.Show("Phiên làm việc của bạn đã hết hạn (401 Unauthorized). Vui lòng đăng nhập lại để tiếp tục thao tác.",
            "Hết hạn phiên làm việc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private async Task InitializeRealtimeAsync()
    {
        try
        {
            _courtRealtimeClient = new CourtRealtimeClient();
            _courtRealtimeClient.OnCellUpdated += HandleRealtimeCellUpdated;
            _courtRealtimeClient.OnConnectionStateChanged += HandleRealtimeConnectionChanged;
            await _courtRealtimeClient.StartAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi khởi động SignalR: {ex.Message}");
        }
    }

    private void HandleRealtimeConnectionChanged(bool isConnected, string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => HandleRealtimeConnectionChanged(isConnected, message)));
            return;
        }

        if (isConnected)
        {
            _badgeShiftStatus.SetStatus("Available", "Online");
        }
        else
        {
            _badgeShiftStatus.SetStatus("Maintenance", "Offline");
        }
    }

    private void HandleRealtimeCellUpdated(CourtTimeSlotCellDto updatedCell)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => HandleRealtimeCellUpdated(updatedCell)));
            return;
        }

        if (_fullMatrixData == null) return;

        var targetRow = _fullMatrixData.Rows.FirstOrDefault(r => r.CourtId == updatedCell.CourtId);
        if (targetRow != null)
        {
            var targetCell = targetRow.Cells.FirstOrDefault(c => c.TimeSlotId == updatedCell.TimeSlotId);
            if (targetCell != null)
            {
                targetCell.VisualStatus = updatedCell.VisualStatus;
                if (!string.IsNullOrEmpty(updatedCell.BookingCode)) targetCell.BookingCode = updatedCell.BookingCode;
                if (!string.IsNullOrEmpty(updatedCell.CustomerName)) targetCell.CustomerName = updatedCell.CustomerName;
                if (!string.IsNullOrEmpty(updatedCell.CustomerPhone)) targetCell.CustomerPhone = updatedCell.CustomerPhone;
                if (updatedCell.Price > 0) targetCell.Price = updatedCell.Price;

                ApplyCourtMatrixFilter();
                _lblMatrixStatusInfo.Text = $"⚡ [Realtime] Ô sân {targetRow.CourtName} ({targetCell.TimeSlotDisplayName}) vừa cập nhật sang trạng thái: {updatedCell.VisualStatus}";
            }
        }
    }

    // =========================================================================
    // 1. DỰNG LAYOUT TỔNG THỂ (SIDEBAR - HEADER - CONTENT)
    // =========================================================================

    private void BuildMainLayout()
    {
        float factor = DpiHelper.GetScaleFactor(this);

        SuspendLayout();

        // ---------------------------------------------------------------------
        // 1. SIDEBAR (BÊN TRÁI - CHIẾM TRỌN 100% CHIỀU CAO TỪ ĐỈNH ĐẾN ĐÁY)
        // ---------------------------------------------------------------------
        int sidebarWidth = DpiHelper.Scale(240, factor);
        _pnlSidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = sidebarWidth,
            BackColor = UITheme.DarkNavy,
            Padding = new Padding(0)
        };
        BuildSidebar(factor);

        // ---------------------------------------------------------------------
        // 2. MAIN WRAPPER (CHỨA HEADER & CONTENT BÊN PHẢI SIDEBAR)
        // ---------------------------------------------------------------------
        _pnlMainWrapper = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.LightBackground
        };

        // 3. HEADER (NẰM Ở ĐỈNH CỦA MAIN WRAPPER)
        int headerHeight = DpiHelper.Scale(72, factor);
        _pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = headerHeight,
            BackColor = Color.White
        };
        _pnlHeader.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawLine(borderPen, 0, _pnlHeader.Height - 1, _pnlHeader.Width, _pnlHeader.Height - 1);
        };
        BuildHeader(factor);

        // 4. CONTENT (BÊN DƯỚI HEADER)
        _pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.LightBackground
        };

        BuildDashboardPage(factor);
        BuildSchedulePage(factor);
        BuildBookingsPage(factor);
        BuildSettingsPage(factor);

        _pnlContent.Controls.Add(_pnlDashboardView);
        _pnlContent.Controls.Add(_pnlScheduleView);
        _pnlContent.Controls.Add(_pnlBookingsView);
        _pnlContent.Controls.Add(_pnlSettingsView);

        // Ráp Header và Content vào MainWrapper
        _pnlMainWrapper.Controls.Add(_pnlContent); // Fill
        _pnlMainWrapper.Controls.Add(_pnlHeader);  // Top
        _pnlHeader.SendToBack();
        _pnlContent.BringToFront();

        // Ráp Sidebar và MainWrapper lên Form chính
        Controls.Add(_pnlMainWrapper); // Fill
        Controls.Add(_pnlSidebar);     // Left (Full Height)
        _pnlSidebar.SendToBack();
        _pnlMainWrapper.BringToFront();

        ResumeLayout(false);
        PerformLayout();
    }

    // =========================================================================
    // 2. XÂY DỰNG SIDEBAR
    // =========================================================================

    private void BuildSidebar(float factor)
    {
        // 1. BRAND PANEL (TOP)
        int brandHeight = DpiHelper.Scale(80, factor);
        _pnlBrand = new Panel
        {
            Dock = DockStyle.Top,
            Height = brandHeight,
            BackColor = UITheme.DarkNavy,
            Padding = new Padding(DpiHelper.Scale(16, factor), DpiHelper.Scale(14, factor), DpiHelper.Scale(16, factor), DpiHelper.Scale(10, factor))
        };
        _pnlBrand.Paint += (s, e) =>
        {
            using var linePen = new Pen(UITheme.NavySurface, 1f);
            e.Graphics.DrawLine(linePen, DpiHelper.Scale(16, factor), _pnlBrand.Height - 1, _pnlBrand.Width - DpiHelper.Scale(16, factor), _pnlBrand.Height - 1);
        };

        var brandIcon = new IconPictureBox
        {
            IconChar = IconChar.Trophy,
            IconColor = UITheme.Primary,
            IconSize = DpiHelper.Scale(32, factor),
            Size = new Size(DpiHelper.Scale(36, factor), DpiHelper.Scale(36, factor)),
            Location = new Point(DpiHelper.Scale(16, factor), DpiHelper.Scale(20, factor)),
            BackColor = Color.Transparent
        };

        var lblBrandTitle = new Label
        {
            Text = "SPORTCHAIN VN",
            UseMnemonic = false,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", Math.Max(10f, 11.5f * factor), FontStyle.Bold),
            Location = new Point(DpiHelper.Scale(56, factor), DpiHelper.Scale(17, factor)),
            AutoSize = true
        };

        var lblBrandSubtitle = new Label
        {
            Text = "Hệ Thống Quản Lý Sân",
            UseMnemonic = false,
            ForeColor = UITheme.TextMuted,
            Font = new Font("Segoe UI", Math.Max(7.5f, 8.5f * factor), FontStyle.Regular),
            Location = new Point(DpiHelper.Scale(57, factor), DpiHelper.Scale(39, factor)),
            AutoSize = true
        };

        _pnlBrand.Controls.AddRange(new Control[] { brandIcon, lblBrandTitle, lblBrandSubtitle });

        // 2. MENU BUTTONS CONTAINER
        var pnlNavMenu = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.DarkNavy,
            Padding = new Padding(DpiHelper.Scale(12, factor), DpiHelper.Scale(14, factor), DpiHelper.Scale(12, factor), DpiHelper.Scale(14, factor)),
            AutoScroll = true
        };

        int btnHeight = DpiHelper.Scale(46, factor);
        int btnSpacing = DpiHelper.Scale(6, factor);

        _btnNavDashboard = CreateNavButton("  Tổng quan & Thống kê", IconChar.ChartPie, factor, btnHeight);
        _btnNavDashboard.Click += (s, e) => NavigateTo(0);

        _btnNavSchedule = CreateNavButton("  Sơ đồ sân trực quan", IconChar.CalendarDays, factor, btnHeight);
        _btnNavSchedule.Click += (s, e) => NavigateTo(1);

        _btnNavBookings = CreateNavButton("  Quản lý đặt sân", IconChar.Receipt, factor, btnHeight);
        _btnNavBookings.Click += (s, e) => NavigateTo(2);

        _btnNavSettings = CreateNavButton("  Cài đặt & Chi nhánh", IconChar.Gear, factor, btnHeight);
        _btnNavSettings.Click += (s, e) => NavigateTo(3);

        // Xếp các nút từ trên xuống dưới
        _btnNavSettings.Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(14, factor) + 3 * (btnHeight + btnSpacing));
        _btnNavBookings.Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(14, factor) + 2 * (btnHeight + btnSpacing));
        _btnNavSchedule.Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(14, factor) + 1 * (btnHeight + btnSpacing));
        _btnNavDashboard.Location = new Point(DpiHelper.Scale(12, factor), DpiHelper.Scale(14, factor));

        pnlNavMenu.Controls.AddRange(new Control[] { _btnNavDashboard, _btnNavSchedule, _btnNavBookings, _btnNavSettings });

        // 3. FOOTER INFO
        int footerHeight = DpiHelper.Scale(64, factor);
        var pnlSidebarFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = footerHeight,
            BackColor = UITheme.DarkNavy,
            Padding = new Padding(DpiHelper.Scale(16, factor), DpiHelper.Scale(10, factor), DpiHelper.Scale(16, factor), DpiHelper.Scale(10, factor))
        };
        pnlSidebarFooter.Paint += (s, e) =>
        {
            using var linePen = new Pen(UITheme.NavySurface, 1f);
            e.Graphics.DrawLine(linePen, DpiHelper.Scale(16, factor), 0, pnlSidebarFooter.Width - DpiHelper.Scale(16, factor), 0);
        };

        _lblSidebarFooter = new Label
        {
            Text = "Chi nhánh: Quận 7 - TP.HCM\nSportChain v1.0 • .NET 8 WinForms",
            UseMnemonic = false,
            ForeColor = UITheme.TextMuted,
            Font = new Font("Segoe UI", Math.Max(7f, 8f * factor), FontStyle.Regular),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlSidebarFooter.Controls.Add(_lblSidebarFooter);

        _pnlSidebar.Controls.Add(pnlNavMenu);
        _pnlSidebar.Controls.Add(pnlSidebarFooter);
        _pnlSidebar.Controls.Add(_pnlBrand);

        _pnlBrand.SendToBack();
        pnlSidebarFooter.SendToBack();
        pnlNavMenu.BringToFront();
    }

    private ModernButton CreateNavButton(string text, IconChar icon, float factor, int height)
    {
        return new ModernButton
        {
            Text = text,
            IconChar = icon,
            IconColor = UITheme.TextMuted,
            IconSize = DpiHelper.Scale(18, factor),
            TextAlign = ContentAlignment.MiddleLeft,
            ImageAlign = ContentAlignment.MiddleLeft,
            BackgroundColor = Color.Transparent,
            HoverColor = UITheme.NavySurface,
            ForeColor = UITheme.TextLight,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            CornerRadius = 8,
            Size = new Size(DpiHelper.Scale(216, factor), height),
            Cursor = Cursors.Hand
        };
    }

    // =========================================================================
    // 3. XÂY DỰNG HEADER
    // =========================================================================

    private void BuildHeader(float factor)
    {
        int headerHeight = DpiHelper.Scale(72, factor);

        // Bên Phải: Nút làm mới + Thông tin ca trực (Dùng FlowLayoutPanel chống chồng chéo, tự co dãn)
        var rightContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, (headerHeight - DpiHelper.Scale(36, factor)) / 2, DpiHelper.Scale(20, factor), 0)
        };

        int btnWidth = DpiHelper.Scale(110, factor);
        int btnHeight = DpiHelper.Scale(36, factor);

        _btnRefresh = new ModernButton
        {
            Text = " Làm mới",
            IconChar = IconChar.Rotate,
            IconColor = UITheme.PrimaryDark,
            IconSize = DpiHelper.Scale(15, factor),
            BackgroundColor = UITheme.PrimaryLight,
            HoverColor = ColorTranslator.FromHtml("#A7F3D0"),
            ForeColor = UITheme.PrimaryDark,
            CornerRadius = 8,
            Size = new Size(btnWidth, btnHeight),
            Margin = new Padding(0, 0, DpiHelper.Scale(18, factor), 0),
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _btnRefresh.Click += async (s, e) =>
        {
            await LoadMatrixDataAsync();
            await LoadBookingsDataAsync();
            MessageBox.Show("Dữ liệu ca sân và thống kê vừa được đồng bộ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        _lblUserShift = new Label
        {
            Text = "👤 Lễ tân Ca 1 (Huỳnh Tấn Phát)",
            UseMnemonic = false,
            ForeColor = UITheme.TextPrimary,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, DpiHelper.Scale(8, factor), DpiHelper.Scale(10, factor), 0)
        };

        int badgeHeight = DpiHelper.Scale(26, factor);
        _badgeShiftStatus = new StatusBadge
        {
            Size = new Size(DpiHelper.Scale(80, factor), badgeHeight),
            Margin = new Padding(0, DpiHelper.Scale(5, factor), 0, 0),
            ShowDot = true
        };
        _badgeShiftStatus.SetStatus("Available", "Online");

        rightContainer.Controls.AddRange(new Control[] { _btnRefresh, _lblUserShift, _badgeShiftStatus });

        // Bên Trái: Tiêu đề trang & Breadcrumb
        var titleContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        _lblHeaderTitle = new Label
        {
            Text = "📊 TỔNG QUAN & THỐNG KÊ CHI NHÁNH",
            UseMnemonic = false,
            ForeColor = UITheme.TextPrimary,
            Font = new Font("Segoe UI", Math.Max(12f, 13.5f * factor), FontStyle.Bold),
            Location = new Point(DpiHelper.Scale(20, factor), DpiHelper.Scale(12, factor)),
            AutoSize = true
        };

        _lblHeaderSubtitle = new Label
        {
            Text = "Trang chủ  ›  SportChain Quận 7  ›  Bảng điều khiển trung tâm",
            UseMnemonic = false,
            ForeColor = ColorTranslator.FromHtml("#475569"),
            Font = new Font("Segoe UI", Math.Max(9f, 9.8f * factor), FontStyle.Regular),
            Location = new Point(DpiHelper.Scale(20, factor), DpiHelper.Scale(42, factor)),
            AutoSize = true
        };

        titleContainer.Controls.Add(_lblHeaderTitle);
        titleContainer.Controls.Add(_lblHeaderSubtitle);

        _pnlHeader.Controls.Add(titleContainer); // Fill
        _pnlHeader.Controls.Add(rightContainer); // Right
        rightContainer.SendToBack();
        titleContainer.BringToFront();
    }

    // =========================================================================
    // 4. TRANG 1: TỔNG QUAN DASHBOARD
    // =========================================================================

    private void BuildDashboardPage(float factor)
    {
        _pnlDashboardView = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(24, factor))
        };

        // 4 Thẻ MetricCard hàng ngang
        int cardHeight = DpiHelper.Scale(110, factor);
        var metricsTable = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = cardHeight,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };
        for (int i = 0; i < 4; i++)
            metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

        var cardActiveCourts = new MetricCard
        {
            Dock = DockStyle.Fill,
            Title = "SÂN HOẠT ĐỘNG",
            Value = "6 / 6 Sân",
            TrendText = "100% sân sẵn sàng • 0 bảo trì",
            TrendColor = UITheme.Primary,
            Icon = IconChar.Futbol,
            IconColor = UITheme.Primary,
            IconBackgroundColor = UITheme.PrimaryLight,
            Margin = new Padding(0, 0, DpiHelper.Scale(8, factor), 0)
        };

        var cardRevenue = new MetricCard
        {
            Dock = DockStyle.Fill,
            Title = "DOANH THU CA TRỰC",
            Value = "8.450.000 đ",
            TrendText = "+18.5% so với mục tiêu ca",
            TrendColor = UITheme.StatusPendingPayment,
            Icon = IconChar.Coins,
            IconColor = ColorTranslator.FromHtml("#D97706"),
            IconBackgroundColor = ColorTranslator.FromHtml("#FEF3C7"),
            Margin = new Padding(DpiHelper.Scale(4, factor), 0, DpiHelper.Scale(4, factor), 0)
        };

        var cardPending = new MetricCard
        {
            Dock = DockStyle.Fill,
            Title = "CA CHỜ DUYỆT CỌC",
            Value = "3 ca",
            TrendText = "Giữ chỗ tối đa 15 phút",
            TrendColor = UITheme.StatusInUse,
            Icon = IconChar.Clock,
            IconColor = UITheme.StatusInUse,
            IconBackgroundColor = UITheme.StatusInUseLight,
            Margin = new Padding(DpiHelper.Scale(4, factor), 0, DpiHelper.Scale(4, factor), 0)
        };

        var cardOccupancy = new MetricCard
        {
            Dock = DockStyle.Fill,
            Title = "TỶ LỆ LẤP ĐẦY",
            Value = "78.4%",
            TrendText = "Khung giờ vàng đạt 95%",
            TrendColor = ColorTranslator.FromHtml("#2563EB"),
            Icon = IconChar.ChartLine,
            IconColor = ColorTranslator.FromHtml("#2563EB"),
            IconBackgroundColor = ColorTranslator.FromHtml("#DBEAFE"),
            Margin = new Padding(DpiHelper.Scale(8, factor), 0, 0, 0)
        };

        metricsTable.Controls.Add(cardActiveCourts, 0, 0);
        metricsTable.Controls.Add(cardRevenue, 1, 0);
        metricsTable.Controls.Add(cardPending, 2, 0);
        metricsTable.Controls.Add(cardOccupancy, 3, 0);

        // KHU VỰC THAO TÁC NHANH TRONG CA
        var quickActionsCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(110, factor),
            BackColor = Color.White,
            Padding = new Padding(DpiHelper.Scale(18, factor)),
            Margin = new Padding(0, DpiHelper.Scale(16, factor), 0, 0)
        };
        quickActionsCard.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, quickActionsCard.Width - 1, quickActionsCard.Height - 1);
        };

        var lblQuickActionTitle = new Label
        {
            Text = "⚡ THAO TÁC NHANH TRONG CA TRỰC",
            ForeColor = UITheme.TextSecondary,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(24, factor)
        };

        var quickButtonsFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            Padding = new Padding(0, DpiHelper.Scale(6, factor), 0, 0)
        };

        var btnCreateBooking = new ModernButton
        {
            Text = " Tạo đơn đặt sân nhanh",
            IconChar = IconChar.PlusCircle,
            IconColor = Color.White,
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            Size = new Size(DpiHelper.Scale(185, factor), DpiHelper.Scale(40, factor)),
            CornerRadius = 8,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, DpiHelper.Scale(12, factor), 0)
        };
        btnCreateBooking.Click += (s, e) =>
        {
            NavigateTo(1); // Chuyển sang Sơ đồ sân
            MessageBox.Show("Mời bạn chọn ô khung giờ trên ma trận sân để tạo đơn đặt sân mới!", "Đặt sân nhanh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        var btnCheckIn = new ModernButton
        {
            Text = " Check-in nhận sân",
            IconChar = IconChar.UserCheck,
            IconColor = Color.White,
            BackgroundColor = ColorTranslator.FromHtml("#2563EB"),
            HoverColor = ColorTranslator.FromHtml("#1D4ED8"),
            Size = new Size(DpiHelper.Scale(165, factor), DpiHelper.Scale(40, factor)),
            CornerRadius = 8,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, DpiHelper.Scale(12, factor), 0)
        };
        btnCheckIn.Click += (s, e) => OpenPosDialog(initialTab: 0);

        var btnEmergencySwap = new ModernButton
        {
            Text = " Đổi sân / Đổi ca khẩn",
            IconChar = IconChar.RightLeft,
            IconColor = Color.White,
            BackgroundColor = ColorTranslator.FromHtml("#D97706"),
            HoverColor = ColorTranslator.FromHtml("#B45309"),
            Size = new Size(DpiHelper.Scale(175, factor), DpiHelper.Scale(40, factor)),
            CornerRadius = 8,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, DpiHelper.Scale(12, factor), 0)
        };
        btnEmergencySwap.Click += (s, e) => OpenPosDialog(initialTab: 2);

        var btnViewFullSchedule = new ModernButton
        {
            Text = " Xem sơ đồ trực quan",
            IconChar = IconChar.CalendarDays,
            IconColor = Color.White,
            BackgroundColor = UITheme.DarkNavy,
            HoverColor = UITheme.NavySurface,
            Size = new Size(DpiHelper.Scale(170, factor), DpiHelper.Scale(40, factor)),
            CornerRadius = 8,
            Cursor = Cursors.Hand
        };
        btnViewFullSchedule.Click += (s, e) => NavigateTo(1);

        quickButtonsFlow.Controls.AddRange(new Control[] { btnCreateBooking, btnCheckIn, btnEmergencySwap, btnViewFullSchedule });

        quickActionsCard.Controls.Add(quickButtonsFlow);
        quickActionsCard.Controls.Add(lblQuickActionTitle);

        // KHU VỰC THÔNG TIN TÓM TẮT SÂN THI ĐẤU
        var summaryCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(280, factor),
            BackColor = Color.White,
            Padding = new Padding(DpiHelper.Scale(18, factor)),
            Margin = new Padding(0, DpiHelper.Scale(16, factor), 0, 0)
        };
        summaryCard.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, summaryCard.Width - 1, summaryCard.Height - 1);
        };

        var lblSummaryTitle = new Label
        {
            Text = "🏟️ TRẠNG THÁI CÁC SÂN THI ĐẤU HÔM NAY (SPORTCHAIN QUẬN 7)",
            ForeColor = UITheme.TextSecondary,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(26, factor)
        };

        var summaryFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            AutoScroll = true,
            Padding = new Padding(0, DpiHelper.Scale(6, factor), 0, 0)
        };

        var courtSummaries = new[]
        {
            ("Sân 5-A (Bắc)", "S5-01", "Bóng đá 5 người", "Đang đá ca 17:00 • Tiếp theo: FC Tiger (18:00)", "InUse", "Đang đá"),
            ("Sân 5-B (Nam)", "S5-02", "Bóng đá 5 người", "Đã cọc 3 ca tối • Khung 19:00 - 22:00", "Confirmed", "Đã cọc"),
            ("Sân 7-A (Trung tâm)", "S7-01", "Bóng đá 7 người", "Đang đá ca 18:00 • Khách: Đội Doanh Nhân", "InUse", "Đang đá"),
            ("Sân Cầu Lông 1", "CL-01", "Cầu lông thảm cao cấp", "Đã đặt 4 ca • Còn trống ca 20:00", "Confirmed", "Đã cọc"),
            ("Sân Pickleball 1", "PB-01", "Pickleball tiêu chuẩn", "Kín lịch tối • 4 ca liên tiếp", "Confirmed", "Đã cọc"),
            ("Sân Tennis 1", "TN-01", "Tennis ngoài trời", "Chờ cọc ca 16:00 • Đã xác nhận ca 18:00", "PendingPayment", "Chờ cọc")
        };

        foreach (var (cName, cCode, cType, cDesc, status, statusText) in courtSummaries)
        {
            var rowPanel = new Panel
            {
                Size = new Size(DpiHelper.Scale(1100, factor), DpiHelper.Scale(32, factor)),
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, DpiHelper.Scale(4, factor))
            };

            var badge = new StatusBadge
            {
                Size = new Size(DpiHelper.Scale(85, factor), DpiHelper.Scale(24, factor)),
                Location = new Point(0, DpiHelper.Scale(2, factor)),
                ShowDot = true
            };
            badge.SetStatus(status, statusText);

            var lblCourtInfo = new Label
            {
                Text = $"[{cCode}] {cName} ({cType}): {cDesc}",
                Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
                ForeColor = UITheme.TextPrimary,
                Location = new Point(DpiHelper.Scale(96, factor), DpiHelper.Scale(4, factor)),
                AutoSize = true
            };

            rowPanel.Controls.Add(badge);
            rowPanel.Controls.Add(lblCourtInfo);
            summaryFlow.Controls.Add(rowPanel);
        }

        summaryCard.Controls.Add(summaryFlow);
        summaryCard.Controls.Add(lblSummaryTitle);

        // Ghép các khối vào Dashboard Panel
        _pnlDashboardView.Controls.Add(summaryCard);
        _pnlDashboardView.Controls.Add(quickActionsCard);
        _pnlDashboardView.Controls.Add(metricsTable);
    }

    // =========================================================================
    // 5. TRANG 2: SƠ ĐỒ SÂN TRỰC QUAN (COURT MATRIX GRID)
    // =========================================================================

    private void BuildSchedulePage(float factor)
    {
        _pnlScheduleView = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        // 1. THANH ĐIỀU KHIỂN & BỘ LỌC (TOP CONTROL BAR)
        int filterHeight = DpiHelper.Scale(54, factor);
        var filterBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = filterHeight,
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(16, factor), DpiHelper.Scale(9, factor), DpiHelper.Scale(16, factor), DpiHelper.Scale(9, factor))
        };
        filterBar.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawLine(borderPen, 0, filterBar.Height - 1, filterBar.Width, filterBar.Height - 1);
        };

        // Container bộ lọc bên trái dùng FlowLayoutPanel (chống chồng chéo chữ và box nhập liệu)
        var filterFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Padding = new Padding(0)
        };

        // Bộ chọn ngày
        var lblDateIcon = new Label
        {
            Text = "📅 Ngày:",
            UseMnemonic = false,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Margin = new Padding(0, DpiHelper.Scale(7, factor), DpiHelper.Scale(6, factor), 0)
        };

        _dtpDate = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.5f * factor), FontStyle.Regular),
            Size = new Size(DpiHelper.Scale(125, factor), DpiHelper.Scale(32, factor)),
            Margin = new Padding(0, DpiHelper.Scale(2, factor), DpiHelper.Scale(16, factor), 0)
        };
        _dtpDate.ValueChanged += async (s, e) => await LoadMatrixDataAsync();

        // Lọc loại sân
        var lblType = new Label
        {
            Text = "Loại sân:",
            UseMnemonic = false,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Margin = new Padding(0, DpiHelper.Scale(7, factor), DpiHelper.Scale(6, factor), 0)
        };

        _cboCourtTypeFilter = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.5f * factor), FontStyle.Regular),
            Size = new Size(DpiHelper.Scale(160, factor), DpiHelper.Scale(32, factor)),
            Margin = new Padding(0, DpiHelper.Scale(2, factor), DpiHelper.Scale(16, factor), 0)
        };
        _cboCourtTypeFilter.Items.AddRange(new object[]
        {
            "Tất cả loại sân",
            "Bóng đá (Football)",
            "Cầu lông (Badminton)",
            "Pickleball",
            "Tennis"
        });
        _cboCourtTypeFilter.SelectedIndex = 0;
        _cboCourtTypeFilter.SelectedIndexChanged += (s, e) => ApplyCourtMatrixFilter();

        // Thanh tìm kiếm nhanh
        _txtSearchCourt = new SearchTextBox
        {
            PlaceholderText = "Tìm theo tên sân, mã đơn, khách hàng...",
            Size = new Size(DpiHelper.Scale(280, factor), DpiHelper.Scale(34, factor)),
            MinimumSize = new Size(DpiHelper.Scale(240, factor), DpiHelper.Scale(34, factor)),
            Margin = new Padding(0, 0, DpiHelper.Scale(16, factor), 0)
        };
        _txtSearchCourt.SearchSubmitted += (s, query) => ApplyCourtMatrixFilter();

        filterFlow.Controls.AddRange(new Control[] { lblDateIcon, _dtpDate, lblType, _cboCourtTypeFilter, _txtSearchCourt });

        // Chú giải trạng thái (Legend) bên phải
        var legendFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Padding = new Padding(0, DpiHelper.Scale(5, factor), 0, 0)
        };

        var lg1 = new StatusBadge { Size = new Size(DpiHelper.Scale(78, factor), DpiHelper.Scale(24, factor)), ShowDot = true }; lg1.SetStatus("Available", "Trống");
        var lg2 = new StatusBadge { Size = new Size(DpiHelper.Scale(88, factor), DpiHelper.Scale(24, factor)), ShowDot = true }; lg2.SetStatus("PendingPayment", "Chờ cọc");
        var lg3 = new StatusBadge { Size = new Size(DpiHelper.Scale(88, factor), DpiHelper.Scale(24, factor)), ShowDot = true }; lg3.SetStatus("Confirmed", "Đã cọc");
        var lg4 = new StatusBadge { Size = new Size(DpiHelper.Scale(96, factor), DpiHelper.Scale(24, factor)), ShowDot = true }; lg4.SetStatus("InUse", "Đang đá");
        var lg5 = new StatusBadge { Size = new Size(DpiHelper.Scale(85, factor), DpiHelper.Scale(24, factor)), ShowDot = true }; lg5.SetStatus("Maintenance", "Bảo trì");

        legendFlow.Controls.AddRange(new Control[] { lg1, lg2, lg3, lg4, lg5 });

        filterBar.Controls.Add(legendFlow);
        filterBar.Controls.Add(filterFlow);

        // 2. THANH CÔNG CỤ POS ACTION BAR TRÊN ĐỈNH MA TRẬN SÂN (MỤC 7.1)
        int posActionBarHeight = DpiHelper.Scale(46, factor);
        var posActionBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = posActionBarHeight,
            BackColor = Color.White,
            Padding = new Padding(DpiHelper.Scale(16, factor), DpiHelper.Scale(6, factor), DpiHelper.Scale(16, factor), DpiHelper.Scale(6, factor))
        };
        posActionBar.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawLine(borderPen, 0, posActionBar.Height - 1, posActionBar.Width, posActionBar.Height - 1);
        };

        // Nút lọc nhanh theo trạng thái (bên trái)
        var statusFilterFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };

        ModernButton CreateQuickFilterBtn(string text, string filterKey, Color activeBg)
        {
            var btn = new ModernButton
            {
                Text = text,
                Size = new Size(DpiHelper.Scale(105, factor), DpiHelper.Scale(32, factor)),
                CornerRadius = 6,
                BackgroundColor = (_quickStatusFilter == filterKey) ? activeBg : UITheme.LightBackground,
                ForeColor = (_quickStatusFilter == filterKey) ? Color.White : UITheme.TextPrimary,
                Font = new Font("Segoe UI Semibold", Math.Max(8f, 8.8f * factor), FontStyle.Bold),
                Margin = new Padding(0, 0, DpiHelper.Scale(6, factor), 0),
                BorderSize = 1f,
                BorderColor = UITheme.BorderColor,
                UseMnemonic = false
            };
            btn.Click += (s, e) =>
            {
                _quickStatusFilter = filterKey;
                foreach (Control c in statusFilterFlow.Controls)
                {
                    if (c is ModernButton b)
                    {
                        bool isSel = (b == btn);
                        b.BackgroundColor = isSel ? activeBg : UITheme.LightBackground;
                        b.ForeColor = isSel ? Color.White : UITheme.TextPrimary;
                    }
                }
                ApplyCourtMatrixFilter();
            };
            return btn;
        }

        var btnFAll = CreateQuickFilterBtn("Tất cả", "All", UITheme.DarkNavy);
        var btnFAvail = CreateQuickFilterBtn("🟢 Trống", "Available", UITheme.StatusAvailable);
        var btnFConfirmed = CreateQuickFilterBtn("🟠 Sắp check-in", "Confirmed", UITheme.StatusConfirmed);
        var btnFInUse = CreateQuickFilterBtn("🔴 Đang đá", "InUse", UITheme.StatusInUse);
        var btnFPending = CreateQuickFilterBtn("🟡 Chờ cọc", "PendingPayment", UITheme.StatusPendingPayment);

        statusFilterFlow.Controls.AddRange(new Control[] { btnFAll, btnFAvail, btnFConfirmed, btnFInUse, btnFPending });

        // Các nút mở nhanh hộp thoại POS (bên phải)
        var posActionsFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };

        var btnQuickCheckIn = new ModernButton
        {
            Text = "🛎️ Check-in nhanh",
            IconChar = IconChar.UserCheck,
            IconSize = 14,
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(130, factor), DpiHelper.Scale(32, factor)),
            CornerRadius = 6,
            Font = new Font("Segoe UI Semibold", Math.Max(8f, 8.8f * factor), FontStyle.Bold),
            Margin = new Padding(0, 0, DpiHelper.Scale(6, factor), 0),
            UseMnemonic = false
        };
        btnQuickCheckIn.Click += (s, e) => OpenPosDialog(initialTab: 0);

        var btnQuickCheckout = new ModernButton
        {
            Text = "💳 Tất toán POS",
            IconChar = IconChar.CashRegister,
            IconSize = 14,
            BackgroundColor = UITheme.PrimaryDark,
            HoverColor = ColorTranslator.FromHtml("#047857"),
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(125, factor), DpiHelper.Scale(32, factor)),
            CornerRadius = 6,
            Font = new Font("Segoe UI Semibold", Math.Max(8f, 8.8f * factor), FontStyle.Bold),
            Margin = new Padding(0, 0, DpiHelper.Scale(6, factor), 0),
            UseMnemonic = false
        };
        btnQuickCheckout.Click += (s, e) => OpenPosDialog(initialTab: 1);

        var btnQuickSwap = new ModernButton
        {
            Text = "🔄 Đổi sân",
            IconChar = IconChar.ArrowRightArrowLeft,
            IconSize = 14,
            BackgroundColor = UITheme.StatusConfirmed,
            HoverColor = ColorTranslator.FromHtml("#EA580C"),
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(105, factor), DpiHelper.Scale(32, factor)),
            CornerRadius = 6,
            Font = new Font("Segoe UI Semibold", Math.Max(8f, 8.8f * factor), FontStyle.Bold),
            Margin = new Padding(0, 0, DpiHelper.Scale(6, factor), 0),
            UseMnemonic = false
        };
        btnQuickSwap.Click += (s, e) => OpenPosDialog(initialTab: 2);

        var btnQuickCancel = new ModernButton
        {
            Text = "❌ Hủy ca",
            IconChar = IconChar.CalendarXmark,
            IconSize = 14,
            BackgroundColor = ColorTranslator.FromHtml("#EF4444"),
            HoverColor = ColorTranslator.FromHtml("#DC2626"),
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(100, factor), DpiHelper.Scale(32, factor)),
            CornerRadius = 6,
            Font = new Font("Segoe UI Semibold", Math.Max(8f, 8.8f * factor), FontStyle.Bold),
            UseMnemonic = false
        };
        btnQuickCancel.Click += (s, e) => OpenPosDialog(initialTab: 3);

        posActionsFlow.Controls.AddRange(new Control[] { btnQuickCheckIn, btnQuickCheckout, btnQuickSwap, btnQuickCancel });

        posActionBar.Controls.Add(posActionsFlow);
        posActionBar.Controls.Add(statusFilterFlow);

        // 3. THANH THÔNG TIN TƯƠNG TÁC DƯỚI ĐÁY (BOTTOM STATUS BAR)
        int statusBarHeight = DpiHelper.Scale(38, factor);
        var statusBar = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = statusBarHeight,
            BackColor = UITheme.NavySurface,
            Padding = new Padding(DpiHelper.Scale(16, factor), 0, DpiHelper.Scale(16, factor), 0)
        };

        _lblMatrixStatusInfo = new Label
        {
            Text = "💡 Nhấp vào ô ca sân để xem thông tin chi tiết. Nhấp đúp chuột để mở ngay hộp thoại thao tác Quầy POS.",
            ForeColor = UITheme.TextLight,
            Font = new Font("Segoe UI", Math.Max(8f, 9f * factor), FontStyle.Regular),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        statusBar.Controls.Add(_lblMatrixStatusInfo);

        // 4. MA TRẬN SÂN (COURT MATRIX GRID)
        _courtMatrixGrid = new CourtMatrixGrid
        {
            Dock = DockStyle.Fill
        };

        _courtMatrixGrid.CellClicked += (s, e) =>
        {
            string cust = !string.IsNullOrWhiteSpace(e.Cell.CustomerName) ? $" • Khách: {e.Cell.CustomerName}" : "";
            string code = !string.IsNullOrWhiteSpace(e.Cell.BookingCode) ? $" • Mã: {e.Cell.BookingCode}" : "";
            _lblMatrixStatusInfo.Text = $"👉 Đã chọn: {e.Row.CourtName} | Ca: {e.TimeSlot.DisplayName} | Trạng thái: {e.Cell.VisualStatus} | Giá: {e.Cell.Price:N0} đ{cust}{code}";
        };

        // Đấu nối Hook Sự kiện POS (Mục 7.1)
        _courtMatrixGrid.PosActionRequested += (s, e) =>
        {
            int tab = 0;
            if (string.Equals(e.Cell.VisualStatus, "InUse", StringComparison.OrdinalIgnoreCase)) tab = 1; // Checkout
            else if (string.Equals(e.Cell.VisualStatus, "Confirmed", StringComparison.OrdinalIgnoreCase)) tab = 0; // Checkin
            else if (string.Equals(e.Cell.VisualStatus, "PendingPayment", StringComparison.OrdinalIgnoreCase)) tab = 0;

            OpenPosDialog(e.Cell, e.Row, e.TimeSlot, tab);
        };

        _courtMatrixGrid.BookingRequested += (s, e) =>
        {
            _lblMatrixStatusInfo.Text = $"⚡ [BTL-20 Hook] Đã kích hoạt hook yêu cầu đặt sân: {e.Row.CourtName} | Ca: {e.TimeSlot.DisplayName} ({e.Cell.Price:N0} đ). Sẵn sàng nhận Popup đặt sân của BTL-20.";
        };

        _courtMatrixGrid.CellDoubleClicked += (s, e) =>
        {
            string msg = $"🏟️ THÔNG TIN CHI TIẾT CA SÂN THỂ THAO:\n\n" +
                         $"• Sân thi đấu: {e.Row.CourtName} ({e.Row.CourtTypeName})\n" +
                         $"• Khung giờ ca: {e.TimeSlot.DisplayName} (Ca {e.TimeSlot.SlotIndex})\n" +
                         $"• Trạng thái hiện tại: {e.Cell.VisualStatus}\n" +
                         $"• Đơn giá niêm yết: {e.Cell.Price:N0} VNĐ\n" +
                         $"• Khách hàng đặt: {e.Cell.CustomerName ?? "Chưa có"}\n" +
                         $"• Số điện thoại: {e.Cell.CustomerPhone ?? "Chưa có"}\n" +
                         $"• Mã đơn booking: {e.Cell.BookingCode ?? "Chưa có"}";

            MessageBox.Show(msg, "Chi Tiết Ca Sân", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        _pnlScheduleView.Controls.Add(_courtMatrixGrid);
        _pnlScheduleView.Controls.Add(posActionBar);
        _pnlScheduleView.Controls.Add(filterBar);
        _pnlScheduleView.Controls.Add(statusBar);
    }

    // =========================================================================
    // 6. TRANG 3: QUẢN LÝ ĐẶT SÂN & LỊCH SỬ HÓA ĐƠN (MỤC 7.3)
    // =========================================================================

    private void BuildBookingsPage(float factor)
    {
        _pnlBookingsView = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.LightBackground
        };

        // 1. THANH TÌM KIẾM & BỘ LỌC HÓA ĐƠN
        int topBarHeight = DpiHelper.Scale(60, factor);
        var filterBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = topBarHeight,
            BackColor = Color.White,
            Padding = new Padding(DpiHelper.Scale(16, factor), DpiHelper.Scale(12, factor), DpiHelper.Scale(16, factor), DpiHelper.Scale(12, factor))
        };
        filterBar.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawLine(borderPen, 0, filterBar.Height - 1, filterBar.Width, filterBar.Height - 1);
        };

        var leftFilterFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };

        var lblDate = new Label
        {
            Text = "📅 Ngày:",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.2f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Margin = new Padding(0, DpiHelper.Scale(7, factor), DpiHelper.Scale(6, factor), 0),
            UseMnemonic = false
        };

        _dtpBookingsDate = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = DateTime.Today,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
            Size = new Size(DpiHelper.Scale(125, factor), DpiHelper.Scale(32, factor)),
            Margin = new Padding(0, DpiHelper.Scale(2, factor), DpiHelper.Scale(14, factor), 0)
        };
        _dtpBookingsDate.ValueChanged += async (s, e) => await LoadBookingsDataAsync();

        var lblStatus = new Label
        {
            Text = "Trạng thái:",
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.2f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Margin = new Padding(0, DpiHelper.Scale(7, factor), DpiHelper.Scale(6, factor), 0),
            UseMnemonic = false
        };

        _cboBookingStatusFilter = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
            Size = new Size(DpiHelper.Scale(175, factor), DpiHelper.Scale(32, factor)),
            Margin = new Padding(0, DpiHelper.Scale(2, factor), DpiHelper.Scale(14, factor), 0)
        };
        _cboBookingStatusFilter.Items.AddRange(new object[]
        {
            "Tất cả trạng thái",
            "Chờ cọc (Pending)",
            "Đã cọc 30% (Confirmed)",
            "Đang đá (InUse)",
            "Đã tất toán 100% (Completed)",
            "Đã hủy (Cancelled)"
        });
        _cboBookingStatusFilter.SelectedIndex = 0;
        _cboBookingStatusFilter.SelectedIndexChanged += (s, e) => FilterBookingsGrid();

        _txtSearchBookings = new SearchTextBox
        {
            PlaceholderText = "Tìm theo mã đơn, khách hàng, SĐT...",
            Size = new Size(DpiHelper.Scale(280, factor), DpiHelper.Scale(34, factor)),
            MinimumSize = new Size(DpiHelper.Scale(240, factor), DpiHelper.Scale(34, factor)),
            Margin = new Padding(0, 0, DpiHelper.Scale(14, factor), 0)
        };
        _txtSearchBookings.SearchSubmitted += (s, q) => FilterBookingsGrid();

        leftFilterFlow.Controls.AddRange(new Control[] { lblDate, _dtpBookingsDate, lblStatus, _cboBookingStatusFilter, _txtSearchBookings });

        // Nút hành động bên phải
        var rightActionFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };

        var btnOpenPos = new ModernButton
        {
            Text = "Mở Quầy POS",
            IconChar = IconChar.CashRegister,
            IconSize = 16,
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(140, factor), DpiHelper.Scale(36, factor)),
            CornerRadius = 6,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.2f * factor), FontStyle.Bold),
            Margin = new Padding(0, 0, DpiHelper.Scale(8, factor), 0),
            UseMnemonic = false
        };
        btnOpenPos.Click += (s, e) => OpenPosDialog();

        var btnPrintReceipt = new ModernButton
        {
            Text = "In Hóa Đơn Tạm",
            IconChar = IconChar.Print,
            IconSize = 16,
            BackgroundColor = UITheme.NavySurface,
            HoverColor = UITheme.DarkNavy,
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(150, factor), DpiHelper.Scale(36, factor)),
            CornerRadius = 6,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.2f * factor), FontStyle.Bold),
            UseMnemonic = false
        };
        btnPrintReceipt.Click += (s, e) => ShowSelectedInvoicePreview();

        rightActionFlow.Controls.AddRange(new Control[] { btnOpenPos, btnPrintReceipt });

        filterBar.Controls.Add(rightActionFlow);
        filterBar.Controls.Add(leftFilterFlow);

        // 2. THANH THỐNG KÊ TỔNG KẾT DƯỚI ĐÁY
        int bottomHeight = DpiHelper.Scale(38, factor);
        var bottomStatus = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = bottomHeight,
            BackColor = UITheme.NavySurface,
            Padding = new Padding(DpiHelper.Scale(16, factor), 0, DpiHelper.Scale(16, factor), 0)
        };

        _lblBookingsSummary = new Label
        {
            Text = "📊 Đang tải danh sách đơn đặt sân...",
            Font = new Font("Segoe UI", Math.Max(8f, 9f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextLight,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        bottomStatus.Controls.Add(_lblBookingsSummary);

        // 3. KHUNG BẢNG DỮ LIỆU & EMPTY STATE
        var pnlGridContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(DpiHelper.Scale(16, factor)),
            BackColor = UITheme.LightBackground
        };

        _dgvBookings = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false,
            GridColor = UITheme.BorderColor
        };
        _dgvBookings.ColumnHeadersDefaultCellStyle.BackColor = UITheme.LightBackground;
        _dgvBookings.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextPrimary;
        _dgvBookings.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.2f * factor), FontStyle.Bold);
        _dgvBookings.ColumnHeadersHeight = DpiHelper.Scale(48, factor);
        _dgvBookings.DefaultCellStyle.Font = new Font("Segoe UI", Math.Max(8.5f, 9f * factor), FontStyle.Regular);
        _dgvBookings.DefaultCellStyle.ForeColor = UITheme.TextPrimary;
        _dgvBookings.DefaultCellStyle.SelectionBackColor = UITheme.PrimaryLight;
        _dgvBookings.DefaultCellStyle.SelectionForeColor = UITheme.TextPrimary;
        _dgvBookings.RowTemplate.Height = DpiHelper.Scale(34, factor);
        _dgvBookings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã Đơn", FillWeight = 12 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Khách Hàng", FillWeight = 20 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số Điện Thoại", FillWeight = 16 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sân Thi Đấu & Khung Giờ", FillWeight = 28 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tổng Tiền", FillWeight = 15 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đã Cọc (30%)", FillWeight = 15 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Còn Lại", FillWeight = 15 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng Thái", FillWeight = 24 });
        _dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày Đặt", FillWeight = 15 });

        _dgvBookings.CellFormatting += (s, e) =>
        {
            if (e.ColumnIndex == 7 && e.Value != null) // Trạng thái
            {
                string val = e.Value.ToString() ?? "";
                if (val.Contains("thi đấu") || val.Contains("InUse") || val.Contains("Đang đá"))
                {
                    e.CellStyle!.ForeColor = UITheme.StatusInUse;
                    e.CellStyle!.Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
                }
                else if (val.Contains("Đã cọc") || val.Contains("Confirmed"))
                {
                    e.CellStyle!.ForeColor = UITheme.StatusConfirmed;
                    e.CellStyle!.Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
                }
                else if (val.Contains("tất toán") || val.Contains("Completed"))
                {
                    e.CellStyle!.ForeColor = UITheme.StatusAvailable;
                    e.CellStyle!.Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
                }
                else if (val.Contains("hủy") || val.Contains("Cancelled"))
                {
                    e.CellStyle!.ForeColor = UITheme.TextMuted;
                }
                else if (val.Contains("Chờ cọc") || val.Contains("Holding"))
                {
                    e.CellStyle!.ForeColor = UITheme.StatusPendingPayment;
                    e.CellStyle!.Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
                }
            }
        };

        _dgvBookings.CellDoubleClick += (s, e) =>
        {
            if (e.RowIndex >= 0 && e.RowIndex < _dgvBookings.Rows.Count)
            {
                var booking = _dgvBookings.Rows[e.RowIndex].Tag as BookingDto;
                if (booking != null)
                {
                    if (booking.BookingStatus == BookingStatus.Completed)
                        ShowSelectedInvoicePreview();
                    else
                        OpenPosDialog(null, null, null, booking.BookingStatus == BookingStatus.CheckedIn ? 1 : 0);
                }
            }
        };

        _emptyBookingsState = new EmptyStateControl
        {
            Dock = DockStyle.Fill,
            Icon = IconChar.Receipt,
            Title = "Không tìm thấy đơn đặt sân nào",
            Description = "Không có đơn đặt sân nào khớp với từ khóa tìm kiếm hoặc ngày được chọn.\nBạn có thể tạo đơn đặt mới trực tiếp tại Quầy POS.",
            ActionButtonText = "Mở Quầy POS Tạo Đơn Mới",
            ShowActionButton = true,
            Visible = false
        };
        _emptyBookingsState.ActionClick += (s, e) => OpenPosDialog();

        pnlGridContainer.Controls.Add(_dgvBookings);
        pnlGridContainer.Controls.Add(_emptyBookingsState);

        _pnlBookingsView.Controls.Add(pnlGridContainer);
        _pnlBookingsView.Controls.Add(filterBar);
        _pnlBookingsView.Controls.Add(bottomStatus);
    }

    // =========================================================================
    // 7. TRANG 4: CÀI ĐẶT & CHI NHÁNH
    // =========================================================================

    private void BuildSettingsPage(float factor)
    {
        _pnlSettingsView = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(30, factor))
        };

        var emptySettings = new EmptyStateControl
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(360, factor),
            Icon = IconChar.Sliders,
            Title = "Cấu Hình Chi Nhánh & Bảng Giá Khung Giờ Vàng",
            Description = "Mô-đun thiết lập giá sân cao điểm, giờ hoạt động từ 06:00 đến 23:00 và phân quyền nhân viên ca trực.\nHệ thống đang vận hành tại SportChain Chi nhánh Quận 7.",
            ActionButtonText = "Làm mới cấu hình chi nhánh",
            ShowActionButton = true
        };
        emptySettings.ActionClick += (s, e) =>
        {
            MessageBox.Show("Cấu hình chi nhánh SportChain Quận 7 đang ở trạng thái tối ưu nhất!", "Cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        _pnlSettingsView.Controls.Add(emptySettings);
    }

    // =========================================================================
    // 8. ĐIỀU HƯỚNG VÀ CHUYỂN TRANG
    // =========================================================================

    private void NavigateTo(int index)
    {
        _currentNavigationIndex = index;

        // Reset màu sắc tất cả các nút Sidebar
        ResetNavButton(_btnNavDashboard, IconChar.ChartPie);
        ResetNavButton(_btnNavSchedule, IconChar.CalendarDays);
        ResetNavButton(_btnNavBookings, IconChar.Receipt);
        ResetNavButton(_btnNavSettings, IconChar.Gear);

        // Ẩn tất cả các view
        _pnlDashboardView.Visible = false;
        _pnlScheduleView.Visible = false;
        _pnlBookingsView.Visible = false;
        _pnlSettingsView.Visible = false;

        switch (index)
        {
            case 0:
                SetActiveNavButton(_btnNavDashboard);
                _pnlDashboardView.Visible = true;
                _pnlDashboardView.BringToFront();
                _lblHeaderTitle.Text = "📊 TỔNG QUAN & THỐNG KÊ CHI NHÁNH";
                _lblHeaderSubtitle.Text = "Trang chủ  ›  SportChain Quận 7  ›  Bảng điều khiển trung tâm";
                break;

            case 1:
                SetActiveNavButton(_btnNavSchedule);
                _pnlScheduleView.Visible = true;
                _pnlScheduleView.BringToFront();
                _lblHeaderTitle.Text = "🏟️ SƠ ĐỒ MA TRẬN SÂN THỂ THAO TRỰC QUAN";
                _lblHeaderSubtitle.Text = "Sơ đồ sân trực quan  ›  SportChain Quận 7  ›  Lưới phân bổ ca trực quan";
                break;

            case 2:
                SetActiveNavButton(_btnNavBookings);
                _pnlBookingsView.Visible = true;
                _pnlBookingsView.BringToFront();
                _lblHeaderTitle.Text = "📑 QUẢN LÝ ĐẶT SÂN & HÓA ĐƠN CA TRỰC";
                _lblHeaderSubtitle.Text = "Quản lý đặt sân  ›  SportChain Quận 7  ›  Hóa đơn & Đặt cọc";
                _ = LoadBookingsDataAsync();
                break;

            case 3:
                SetActiveNavButton(_btnNavSettings);
                _pnlSettingsView.Visible = true;
                _pnlSettingsView.BringToFront();
                _lblHeaderTitle.Text = "⚙️ CÀI ĐẶT HỆ THỐNG & CHI NHÁNH";
                _lblHeaderSubtitle.Text = "Cài đặt & Chi nhánh  ›  SportChain Quận 7  ›  Cấu hình giá & Khung giờ";
                break;
        }
    }

    private void SetActiveNavButton(ModernButton btn)
    {
        btn.BackgroundColor = UITheme.Primary;
        btn.HoverColor = UITheme.PrimaryDark;
        btn.ForeColor = Color.White;
        btn.IconColor = Color.White;
    }

    private void ResetNavButton(ModernButton btn, IconChar icon)
    {
        btn.BackgroundColor = Color.Transparent;
        btn.HoverColor = UITheme.NavySurface;
        btn.ForeColor = UITheme.TextLight;
        btn.IconColor = UITheme.TextMuted;
    }

    // =========================================================================
    // 9. NẠP DỮ LIỆU MA TRẬN SÂN (API NETWORK & FALLBACK MOCK DATA)
    // =========================================================================

    private async Task LoadMatrixDataAsync()
    {
        var selectedDate = _dtpDate.Value.Date;
        int branchId = UserSession.BranchId;

        if (_lblMatrixStatusInfo != null)
        {
            _lblMatrixStatusInfo.Text = "⏳ Đang kết nối máy chủ WebApi và nạp lịch sân...";
        }

        Cursor = Cursors.WaitCursor;

        try
        {
            // 1. Thử gọi API lấy dữ liệu thực tế từ máy chủ Backend
            var apiData = await CourtScheduleApiService.GetCourtMatrixAsync(branchId, selectedDate);
            if (apiData != null && apiData.Rows != null && apiData.Rows.Count > 0)
            {
                _fullMatrixData = apiData;
                if (_lblMatrixStatusInfo != null)
                {
                    _lblMatrixStatusInfo.Text = $"🟢 Đã đồng bộ lịch sân trực tiếp từ máy chủ WebApi ({apiData.Rows.Count} sân). Nhấp đúp chuột để xem chi tiết ca.";
                }
            }
            else
            {
                // Máy chủ chưa có dữ liệu ngày này -> Dùng Mock Data
                _fullMatrixData = GenerateMockData(selectedDate);
                if (_lblMatrixStatusInfo != null)
                {
                    _lblMatrixStatusInfo.Text = "ℹ️ Máy chủ chưa có dữ liệu lịch sân cho ngày này. Đang nạp dữ liệu mô phỏng.";
                }
            }
        }
        catch (Exception ex)
        {
            // 2. Cơ chế Fallback an toàn khi Backend offline / mất mạng
            _fullMatrixData = GenerateMockData(selectedDate);
            if (_lblMatrixStatusInfo != null)
            {
                _lblMatrixStatusInfo.Text = $"⚠️ Không thể kết nối WebApi ({ex.Message}). Chuyển sang chế độ Offline Mock Data.";
            }
        }
        finally
        {
            Cursor = Cursors.Default;
            ApplyCourtMatrixFilter();
        }
    }

    private CourtGridResponseDto GenerateMockData(DateTime date)
    {
        var mockResponse = new CourtGridResponseDto
        {
            BranchId = UserSession.BranchId,
            BranchName = UserSession.BranchName,
            SelectedDate = date.Date
        };

        // 17 Khung giờ từ 06:00 đến 23:00 (mỗi ca 60 phút)
        for (int i = 6; i <= 22; i++)
        {
            var start = new TimeSpan(i, 0, 0);
            var end = new TimeSpan(i + 1, 0, 0);
            bool isPeak = (i >= 17 && i <= 21); // Giờ cao điểm: 17h - 22h

            mockResponse.TimeSlots.Add(new TimeSlotDto
            {
                Id = i - 5,
                SlotIndex = i - 5,
                StartTime = start,
                EndTime = end,
                IsPeakHour = isPeak
            });
        }

        // Danh sách 6 Sân thi đấu mẫu với các loại sân khác nhau
        var courtsInfo = new[]
        {
            ("Sân 5-A (Bắc)", "S5-01", CourtType.Football, 180000m, 280000m),
            ("Sân 5-B (Nam)", "S5-02", CourtType.Football, 180000m, 280000m),
            ("Sân 7-A (Trung tâm)", "S7-01", CourtType.Football, 350000m, 500000m),
            ("Sân Cầu Lông 1", "CL-01", CourtType.Badminton, 80000m, 120000m),
            ("Sân Pickleball 1", "PB-01", CourtType.Pickleball, 120000m, 180000m),
            ("Sân Tennis 1", "TN-01", CourtType.Tennis, 150000m, 250000m)
        };

        for (int r = 0; r < courtsInfo.Length; r++)
        {
            var (name, code, type, normalPrice, peakPrice) = courtsInfo[r];

            var row = new CourtGridRowDto
            {
                CourtId = r + 1,
                CourtCode = code,
                CourtName = name,
                CourtType = type
            };

            for (int s = 0; s < mockResponse.TimeSlots.Count; s++)
            {
                var slot = mockResponse.TimeSlots[s];
                decimal price = slot.IsPeakHour ? peakPrice : normalPrice;

                var cell = new CourtTimeSlotCellDto
                {
                    CourtId = row.CourtId,
                    TimeSlotId = slot.Id,
                    TimeSlotDisplayName = slot.DisplayName,
                    BookingDate = date.Date,
                    Price = price,
                    VisualStatus = "Available"
                };

                // Phủ dữ liệu thực tế mô phỏng 5 trạng thái
                if (r == 0) // Sân 5-A
                {
                    if (s == 0) { cell.VisualStatus = "Confirmed"; cell.CustomerName = "CLB Hừng Đông"; cell.CustomerPhone = "0908123456"; cell.BookingCode = "BK-1001"; }
                    else if (s == 11) { cell.VisualStatus = "InUse"; cell.CustomerName = "FC Tiger"; cell.CustomerPhone = "0912345678"; cell.BookingCode = "BK-1002"; }
                    else if (s == 12) { cell.VisualStatus = "InUse"; cell.CustomerName = "FC Tiger"; cell.CustomerPhone = "0912345678"; cell.BookingCode = "BK-1002"; }
                    else if (s == 13) { cell.VisualStatus = "PendingPayment"; cell.BookingCode = "HOLD-882"; }
                    else if (s == 16) { cell.VisualStatus = "Maintenance"; }
                }
                else if (r == 1) // Sân 5-B
                {
                    if (s >= 11 && s <= 13) { cell.VisualStatus = "Confirmed"; cell.CustomerName = "Anh Tuấn Anh"; cell.CustomerPhone = "0977889900"; cell.BookingCode = "BK-1005"; }
                    else if (s == 14) { cell.VisualStatus = "PendingPayment"; cell.BookingCode = "HOLD-771"; }
                }
                else if (r == 2) // Sân 7-A
                {
                    if (s >= 12 && s <= 14) { cell.VisualStatus = "InUse"; cell.CustomerName = "Đội Doanh Nhân"; cell.CustomerPhone = "0933445566"; cell.BookingCode = "BK-1010"; }
                    else if (s == 15) { cell.VisualStatus = "Confirmed"; cell.CustomerName = "FC Bách Khoa"; cell.CustomerPhone = "0944556677"; cell.BookingCode = "BK-1011"; }
                }
                else if (r == 3) // Cầu Lông 1
                {
                    if (s >= 1 && s <= 3) { cell.VisualStatus = "Confirmed"; cell.CustomerName = "CLB Cầu Lông Q7"; cell.CustomerPhone = "0966778899"; cell.BookingCode = "BK-1020"; }
                    else if (s == 16) { cell.VisualStatus = "Maintenance"; }
                }
                else if (r == 4) // Pickleball 1
                {
                    if (s >= 11 && s <= 14) { cell.VisualStatus = "Confirmed"; cell.CustomerName = "Hội Pickleball"; cell.CustomerPhone = "0988112233"; cell.BookingCode = "BK-1030"; }
                }
                else if (r == 5) // Tennis 1
                {
                    if (s == 10) { cell.VisualStatus = "PendingPayment"; cell.BookingCode = "HOLD-990"; }
                    else if (s >= 12 && s <= 13) { cell.VisualStatus = "Confirmed"; cell.CustomerName = "Anh Minh Quân"; cell.CustomerPhone = "0909090909"; cell.BookingCode = "BK-1040"; }
                }

                row.Cells.Add(cell);
            }

            mockResponse.Rows.Add(row);
        }

        return mockResponse;
    }

    private void ApplyCourtMatrixFilter()
    {
        if (_fullMatrixData == null) return;

        var filteredResponse = new CourtGridResponseDto
        {
            BranchId = _fullMatrixData.BranchId,
            BranchName = _fullMatrixData.BranchName,
            SelectedDate = _dtpDate.Value.Date
        };

        foreach (var slot in _fullMatrixData.TimeSlots)
            filteredResponse.TimeSlots.Add(slot);

        int selectedTypeIndex = _cboCourtTypeFilter.SelectedIndex;
        string searchQuery = _txtSearchCourt.Text.Trim().ToLowerInvariant();

        foreach (var row in _fullMatrixData.Rows)
        {
            // Lọc theo loại sân
            bool matchesType = selectedTypeIndex switch
            {
                1 => row.CourtType == CourtType.Football,
                2 => row.CourtType == CourtType.Badminton,
                3 => row.CourtType == CourtType.Pickleball,
                4 => row.CourtType == CourtType.Tennis,
                _ => true // Tất cả
            };

            if (!matchesType) continue;

            // Lọc nhanh theo trạng thái ca sân (POS Action Bar)
            if (!string.IsNullOrEmpty(_quickStatusFilter) && _quickStatusFilter != "All")
            {
                bool matchesStatus = row.Cells.Any(c => string.Equals(c.VisualStatus, _quickStatusFilter, StringComparison.OrdinalIgnoreCase));
                if (!matchesStatus) continue;
            }

            // Lọc theo tìm kiếm từ khóa
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                bool matchesSearch = row.CourtName.ToLowerInvariant().Contains(searchQuery) ||
                                     row.CourtCode.ToLowerInvariant().Contains(searchQuery) ||
                                     row.Cells.Any(c => (!string.IsNullOrEmpty(c.CustomerName) && c.CustomerName.ToLowerInvariant().Contains(searchQuery)) ||
                                                        (!string.IsNullOrEmpty(c.BookingCode) && c.BookingCode.ToLowerInvariant().Contains(searchQuery)));

                if (!matchesSearch) continue;
            }

            filteredResponse.Rows.Add(row);
        }

        _courtMatrixGrid.LoadData(filteredResponse);
    }

    // =========================================================================
    // 10. PHƯƠNG THỨC NGHIỆP VỤ POS & QUẢN LÝ ĐẶT SÂN (BTL-21)
    // =========================================================================

    public void OpenPosDialog(CourtTimeSlotCellDto? cell = null, CourtGridRowDto? row = null, TimeSlotDto? slot = null, int initialTab = 0)
    {
        var allCourts = _fullMatrixData?.Rows ?? new List<CourtGridRowDto>();
        using var dlg = new PosActionDialog(cell, row, slot, initialTab, allCourts);
        if (dlg.ShowDialog(this) == DialogResult.OK && dlg.HasDataChanged)
        {
            ApplyCourtMatrixFilter();
            _ = LoadBookingsDataAsync();
        }
    }

    private async Task LoadBookingsDataAsync()
    {
        var selectedDate = _dtpBookingsDate?.Value.Date ?? DateTime.Today;
        int branchId = UserSession.BranchId;

        try
        {
            _allBookingsData = await BookingApiService.GetBookingsAsync(branchId, selectedDate);
        }
        catch
        {
            _allBookingsData = BookingApiService.GenerateMockBookings(branchId, selectedDate);
        }

        FilterBookingsGrid();
    }

    private void FilterBookingsGrid()
    {
        if (_dgvBookings == null) return;

        string query = _txtSearchBookings?.Text.Trim().ToLowerInvariant() ?? "";
        int statusIdx = _cboBookingStatusFilter?.SelectedIndex ?? 0;

        var filtered = _allBookingsData.Where(b =>
        {
            bool matchQuery = string.IsNullOrWhiteSpace(query) ||
                              b.BookingCode.ToLowerInvariant().Contains(query) ||
                              b.CustomerName.ToLowerInvariant().Contains(query) ||
                              b.CustomerPhone.ToLowerInvariant().Contains(query) ||
                              b.Details.Any(d => d.CourtName.ToLowerInvariant().Contains(query));

            if (!matchQuery) return false;

            return statusIdx switch
            {
                1 => b.BookingStatus == BookingStatus.Holding,
                2 => b.BookingStatus == BookingStatus.Confirmed,
                3 => b.BookingStatus == BookingStatus.CheckedIn,
                4 => b.BookingStatus == BookingStatus.Completed,
                5 => b.BookingStatus == BookingStatus.Cancelled,
                _ => true
            };
        }).ToList();

        _dgvBookings.Rows.Clear();

        foreach (var b in filtered)
        {
            string courts = string.Join(", ", b.Details.Select(d => $"{d.CourtName} ({d.TimeSlotDisplayName})"));
            if (string.IsNullOrEmpty(courts)) courts = "Sân 5-A (Bắc) (17:00 - 18:00)";

            string statusText = b.BookingStatus switch
            {
                BookingStatus.Holding => "Chờ cọc",
                BookingStatus.Confirmed => "Đã cọc 30%",
                BookingStatus.CheckedIn => "Đang thi đấu",
                BookingStatus.Completed => "Đã tất toán",
                BookingStatus.Cancelled => "Đã hủy",
                _ => b.BookingStatus.ToString()
            };

            int rowIdx = _dgvBookings.Rows.Add(
                b.BookingCode,
                b.CustomerName,
                b.CustomerPhone,
                courts,
                b.TotalAmount.ToString("N0") + " đ",
                b.DepositAmount.ToString("N0") + " đ",
                b.RemainingAmount.ToString("N0") + " đ",
                statusText,
                b.CreatedAt.ToString("HH:mm - dd/MM")
            );

            _dgvBookings.Rows[rowIdx].Tag = b;
        }

        bool hasData = filtered.Count > 0;
        _dgvBookings.Visible = hasData;
        _emptyBookingsState.Visible = !hasData;

        decimal totalRevenue = filtered.Where(b => b.BookingStatus != BookingStatus.Cancelled).Sum(b => b.TotalAmount);
        decimal depositCollected = filtered.Where(b => b.BookingStatus != BookingStatus.Cancelled).Sum(b => b.DepositAmount);
        _lblBookingsSummary.Text = $"📊 Tổng số: {filtered.Count} đơn đặt  •  Tổng giá trị ca sân: {totalRevenue:N0} VNĐ  •  Tiền cọc đã thu: {depositCollected:N0} VNĐ";
    }

    private void ShowSelectedInvoicePreview()
    {
        BookingDto? selectedBooking = null;
        if (_dgvBookings.SelectedRows.Count > 0)
        {
            selectedBooking = _dgvBookings.SelectedRows[0].Tag as BookingDto;
        }

        if (selectedBooking == null && _allBookingsData.Count > 0)
        {
            selectedBooking = _allBookingsData[0];
        }

        if (selectedBooking == null)
        {
            MessageBox.Show("Vui lòng chọn một đơn đặt sân trên bảng để xem trước hóa đơn.", "Chưa chọn đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var invoice = new InvoiceDto
        {
            InvoiceCode = $"HD-{DateTime.Today:yyyyMMdd}-{selectedBooking.Id:D3}",
            BookingCode = selectedBooking.BookingCode,
            BranchId = selectedBooking.BranchId,
            BranchName = selectedBooking.BranchName,
            CustomerName = selectedBooking.CustomerName,
            CustomerPhone = selectedBooking.CustomerPhone,
            CourtFee = selectedBooking.TotalAmount,
            ServiceFee = 35000,
            DiscountAmount = 0,
            DepositPaid = selectedBooking.DepositAmount,
            CashGiven = selectedBooking.TotalAmount + 35000 - selectedBooking.DepositAmount,
            PaymentMethod = PaymentMethod.Cash,
            CashierName = UserSession.FullName,
            CreatedAt = DateTime.Now,
            ServiceDetails = new List<BookingServiceDetailDto>
            {
                new() { ServiceItemName = "Nước điện giải Revive", Unit = "Chai", Quantity = 2, UnitPrice = 15000 },
                new() { ServiceItemName = "Khăn lạnh thể thao", Unit = "Cái", Quantity = 1, UnitPrice = 5000 }
            }
        };

        using var preview = new InvoicePreviewDialog(invoice);
        preview.ShowDialog(this);
    }

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        UserSession.OnSessionExpired -= HandleSessionExpired;
        if (_courtRealtimeClient != null)
        {
            await _courtRealtimeClient.DisposeAsync();
            _courtRealtimeClient = null;
        }
    }
}
