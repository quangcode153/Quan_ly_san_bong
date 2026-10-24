using System.ComponentModel;
using System.Drawing;
using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WinForms.Controls.Badges;
using BTLtruc_quan.WinForms.Controls.Buttons;
using BTLtruc_quan.WinForms.Controls.Cards;
using BTLtruc_quan.WinForms.Controls.Common;
using BTLtruc_quan.WinForms.Controls.Grids;
using BTLtruc_quan.WinForms.Controls.Inputs;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Forms;

public partial class ComponentGalleryForm : Form
{
    private Panel _headerPanel = null!;
    private Panel _mainContainer = null!;
    private Panel _controlsTabPanel = null!;
    private Panel _matrixTabPanel = null!;

    private ModernButton _btnTabControls = null!;
    private ModernButton _btnTabMatrix = null!;
    private Label _dpiLabel = null!;

    private CourtMatrixGrid _courtGrid = null!;
    private Label _statusNotificationLabel = null!;

    public ComponentGalleryForm()
    {
        InitializeComponent();
        BuildLayout();
        LoadMockData();
    }

    private void BuildLayout()
    {
        float factor = DpiHelper.GetScaleFactor(this);
        BackColor = UITheme.LightBackground;

        // -----------------------------------------------------------------
        // 1. HEADER PANEL
        // -----------------------------------------------------------------
        int headerHeight = DpiHelper.Scale(68, factor);
        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = headerHeight,
            BackColor = UITheme.DarkNavy,
            Padding = new Padding(DpiHelper.Scale(16, factor), 0, DpiHelper.Scale(16, factor), 0)
        };

        // Logo & Title
        var titleContainer = new Panel
        {
            Dock = DockStyle.Left,
            Width = DpiHelper.Scale(400, factor),
            BackColor = Color.Transparent
        };

        var titleLabel = new Label
        {
            Text = "SPORTCHAIN VN",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", Math.Max(12f, 13.5f * factor), FontStyle.Bold),
            Location = new Point(0, DpiHelper.Scale(12, factor)),
            AutoSize = true
        };

        var subtitleLabel = new Label
        {
            Text = "Hệ sinh thái Custom Controls & Ma trận đặt sân (DPI-Aware)",
            ForeColor = UITheme.TextMuted,
            Font = new Font("Segoe UI", Math.Max(7.5f, 8.5f * factor), FontStyle.Regular),
            Location = new Point(0, DpiHelper.Scale(36, factor)),
            AutoSize = true
        };

        titleContainer.Controls.Add(titleLabel);
        titleContainer.Controls.Add(subtitleLabel);

        // DPI Info Badge (bên phải)
        _dpiLabel = new Label
        {
            Dock = DockStyle.Right,
            ForeColor = UITheme.PrimaryLight,
            Font = new Font("Segoe UI Semibold", Math.Max(8f, 9f * factor), FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight,
            Width = DpiHelper.Scale(240, factor)
        };
        UpdateDpiLabel();

        // Tab Navigation Buttons (ở giữa)
        var tabButtonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(DpiHelper.Scale(20, factor), DpiHelper.Scale(14, factor), 0, 0)
        };

        int btnWidth = DpiHelper.Scale(190, factor);
        int btnHeight = DpiHelper.Scale(38, factor);

        _btnTabControls = new ModernButton
        {
            Text = " 🎨 UI Controls Gallery",
            IconChar = IconChar.Palette,
            IconColor = Color.White,
            IconSize = DpiHelper.Scale(16, factor),
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            CornerRadius = 8,
            Size = new Size(btnWidth, btnHeight),
            Cursor = Cursors.Hand
        };
        _btnTabControls.Click += (s, e) => SwitchTab(true);

        _btnTabMatrix = new ModernButton
        {
            Text = " 🏟️ Ma Trận Sân & Ca",
            IconChar = IconChar.CalendarDays,
            IconColor = UITheme.TextMuted,
            IconSize = DpiHelper.Scale(16, factor),
            BackgroundColor = UITheme.NavySurface,
            HoverColor = UITheme.PrimaryDark,
            CornerRadius = 8,
            Size = new Size(btnWidth, btnHeight),
            Cursor = Cursors.Hand
        };
        _btnTabMatrix.Click += (s, e) => SwitchTab(false);

        tabButtonsPanel.Controls.Add(_btnTabControls);
        tabButtonsPanel.Controls.Add(_btnTabMatrix);

        _headerPanel.Controls.Add(tabButtonsPanel);
        _headerPanel.Controls.Add(titleContainer);
        _headerPanel.Controls.Add(_dpiLabel);

        // -----------------------------------------------------------------
        // 2. MAIN CONTAINER
        // -----------------------------------------------------------------
        _mainContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UITheme.LightBackground
        };

        // Dựng 2 Tabs
        BuildControlsGalleryTab(factor);
        BuildMatrixTab(factor);

        _mainContainer.Controls.Add(_controlsTabPanel);
        _mainContainer.Controls.Add(_matrixTabPanel);

        Controls.Add(_mainContainer);
        Controls.Add(_headerPanel);

        // Mặc định mở Tab 1
        SwitchTab(true);
    }

    private void SwitchTab(bool showControlsTab)
    {
        _controlsTabPanel.Visible = showControlsTab;
        _matrixTabPanel.Visible = !showControlsTab;

        if (showControlsTab)
        {
            _btnTabControls.BackgroundColor = UITheme.Primary;
            _btnTabControls.IconColor = Color.White;
            _btnTabMatrix.BackgroundColor = UITheme.NavySurface;
            _btnTabMatrix.IconColor = UITheme.TextMuted;
        }
        else
        {
            _btnTabMatrix.BackgroundColor = UITheme.Primary;
            _btnTabMatrix.IconColor = Color.White;
            _btnTabControls.BackgroundColor = UITheme.NavySurface;
            _btnTabControls.IconColor = UITheme.TextMuted;
        }
    }

    private void UpdateDpiLabel()
    {
        float factor = DpiHelper.GetScaleFactor(this);
        int percent = (int)Math.Round(factor * 100);
        _dpiLabel.Text = $"🖥️ Màn hình: {DeviceDpi} DPI ({percent}% Scale)";
    }

    // =========================================================================
    // TAB 1: CORE UI CONTROLS GALLERY
    // =========================================================================

    private void BuildControlsGalleryTab(float factor)
    {
        _controlsTabPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(24, factor))
        };

        int sectionMargin = DpiHelper.Scale(16, factor);

        // 1. Dashboard Metrics Section (4 thẻ MetricCard trên 1 dòng)
        var metricsTable = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(116, factor),
            ColumnCount = 4,
            RowCount = 1,
            BackColor = Color.Transparent
        };
        metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        metricsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

        var card1 = new MetricCard
        {
            Title = "SÂN ĐANG THI ĐẤU",
            Value = "12 / 16",
            TrendText = "▲ +2 sân so với ca trước",
            TrendColor = UITheme.Primary,
            Icon = IconChar.Futbol,
            IconColor = UITheme.Primary,
            IconBackgroundColor = UITheme.PrimaryLight,
            Dock = DockStyle.Fill,
            Margin = new Padding(DpiHelper.Scale(4, factor))
        };

        var card2 = new MetricCard
        {
            Title = "DOANH THU CA HIỆN TẠI",
            Value = "4.250.000 đ",
            TrendText = "Đạt 85% chỉ tiêu ngày",
            TrendColor = UITheme.Primary,
            Icon = IconChar.MoneyBillWave,
            IconColor = ColorTranslator.FromHtml("#059669"),
            IconBackgroundColor = ColorTranslator.FromHtml("#D1FAE5"),
            Dock = DockStyle.Fill,
            Margin = new Padding(DpiHelper.Scale(4, factor))
        };

        var card3 = new MetricCard
        {
            Title = "CA CHỜ XÁC NHẬN",
            Value = "5 ca",
            TrendText = "⚠️ Cần duyệt trong 15 phút",
            TrendColor = UITheme.StatusPendingPayment,
            Icon = IconChar.Clock,
            IconColor = UITheme.StatusPendingPayment,
            IconBackgroundColor = UITheme.StatusPendingPaymentLight,
            Dock = DockStyle.Fill,
            Margin = new Padding(DpiHelper.Scale(4, factor))
        };

        var card4 = new MetricCard
        {
            Title = "TỶ LỆ LẤP ĐẦY TRUNG BÌNH",
            Value = "75%",
            TrendText = "▲ +8% so với tuần trước",
            TrendColor = ColorTranslator.FromHtml("#2563EB"),
            Icon = IconChar.ChartPie,
            IconColor = ColorTranslator.FromHtml("#2563EB"),
            IconBackgroundColor = ColorTranslator.FromHtml("#DBEAFE"),
            Dock = DockStyle.Fill,
            Margin = new Padding(DpiHelper.Scale(4, factor))
        };

        metricsTable.Controls.Add(card1, 0, 0);
        metricsTable.Controls.Add(card2, 1, 0);
        metricsTable.Controls.Add(card3, 2, 0);
        metricsTable.Controls.Add(card4, 3, 0);

        // 2. Bảng chia 2 cột cho các Controls nguyên tử
        var detailsTable = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(540, factor),
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = new Padding(0, sectionMargin, 0, 0)
        };
        detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

        // --- CỘT TRÁI (StatusBadges, Buttons, ToggleSwitch) ---
        var leftCard = CreateCardContainer(factor, "BỘ ĐIỀU KHIỂN NGUYÊN TỬ (BADGES, BUTTONS & TOGGLES)");

        // Badges FlowPanel
        var badgeLabel = CreateSubHeaderLabel("1. Trực quan hóa trạng thái (Status Badges):", factor);
        var badgesFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent,
            Margin = new Padding(0, DpiHelper.Scale(4, factor), 0, DpiHelper.Scale(12, factor))
        };

        var b1 = new StatusBadge(); b1.SetStatus("Available", "Sẵn sàng");
        var b2 = new StatusBadge(); b2.SetStatus("PendingPayment", "Chờ đặt cọc");
        var b3 = new StatusBadge(); b3.SetStatus("Confirmed", "Đã xác nhận");
        var b4 = new StatusBadge(); b4.SetStatus("InUse", "Đang thi đấu");
        var b5 = new StatusBadge(); b5.SetStatus("Maintenance", "Bảo trì");

        badgesFlow.Controls.AddRange(new Control[] { b1, b2, b3, b4, b5 });

        // Buttons FlowPanel
        var buttonLabel = CreateSubHeaderLabel("2. Nút bấm phẳng hiện đại (Modern Buttons):", factor);
        var buttonsFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent,
            Margin = new Padding(0, DpiHelper.Scale(4, factor), 0, DpiHelper.Scale(12, factor))
        };

        var btnPrimary = new ModernButton
        {
            Text = " Đặt sân ngay",
            IconChar = IconChar.Plus,
            IconColor = Color.White,
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            Size = new Size(DpiHelper.Scale(130, factor), DpiHelper.Scale(36, factor))
        };

        var btnConfirm = new ModernButton
        {
            Text = " Xác nhận cọc",
            IconChar = IconChar.Check,
            IconColor = Color.White,
            BackgroundColor = UITheme.StatusConfirmed,
            HoverColor = ColorTranslator.FromHtml("#EA580C"),
            Size = new Size(DpiHelper.Scale(135, factor), DpiHelper.Scale(36, factor))
        };

        var btnCancel = new ModernButton
        {
            Text = " Hủy ca",
            IconChar = IconChar.TrashCan,
            IconColor = Color.White,
            BackgroundColor = UITheme.StatusInUse,
            HoverColor = ColorTranslator.FromHtml("#DC2626"),
            Size = new Size(DpiHelper.Scale(110, factor), DpiHelper.Scale(36, factor))
        };

        var btnSecondary = new ModernButton
        {
            Text = " In hóa đơn",
            IconChar = IconChar.Print,
            IconColor = UITheme.TextPrimary,
            BackgroundColor = ColorTranslator.FromHtml("#E2E8F0"),
            HoverColor = ColorTranslator.FromHtml("#CBD5E1"),
            Size = new Size(DpiHelper.Scale(125, factor), DpiHelper.Scale(36, factor))
        };

        buttonsFlow.Controls.AddRange(new Control[] { btnPrimary, btnConfirm, btnCancel, btnSecondary });

        // Toggle Switch FlowPanel
        var toggleLabel = CreateSubHeaderLabel("3. Công tắc bật/tắt phẳng (Toggle Switch):", factor);
        var toggleFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent,
            Margin = new Padding(0, DpiHelper.Scale(4, factor), 0, 0)
        };

        var t1 = new ToggleSwitch { Checked = true };
        var l1 = new Label { Text = "Bật đèn sân", AutoSize = true, Margin = new Padding(DpiHelper.Scale(4, factor), DpiHelper.Scale(4, factor), DpiHelper.Scale(16, factor), 0) };

        var t2 = new ToggleSwitch { Checked = false };
        var l2 = new Label { Text = "Thuê trọng tài", AutoSize = true, Margin = new Padding(DpiHelper.Scale(4, factor), DpiHelper.Scale(4, factor), DpiHelper.Scale(16, factor), 0) };

        var t3 = new ToggleSwitch { Checked = true, ShowText = true, Size = new Size(DpiHelper.Scale(60, factor), DpiHelper.Scale(28, factor)) };
        var l3 = new Label { Text = "Tự động làm mới", AutoSize = true, Margin = new Padding(DpiHelper.Scale(4, factor), DpiHelper.Scale(4, factor), 0, 0) };

        toggleFlow.Controls.AddRange(new Control[] { t1, l1, t2, l2, t3, l3 });

        leftCard.Controls.Add(toggleFlow);
        leftCard.Controls.Add(toggleLabel);
        leftCard.Controls.Add(buttonsFlow);
        leftCard.Controls.Add(buttonLabel);
        leftCard.Controls.Add(badgesFlow);
        leftCard.Controls.Add(badgeLabel);

        // --- CỘT PHẢI (ModernTextBox, SearchTextBox, EmptyState) ---
        var rightCard = CreateCardContainer(factor, "Ô NHẬP LIỆU & GIAO DIỆN TRẠNG THÁI RỖNG");

        var inputLabel = CreateSubHeaderLabel("1. Ô nhập liệu bo góc (Inputs & Search):", factor);

        var txtName = new ModernTextBox
        {
            PlaceholderText = "Nhập họ tên khách hàng...",
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(38, factor),
            Margin = new Padding(0, 0, 0, DpiHelper.Scale(10, factor))
        };

        var txtSearch = new SearchTextBox
        {
            PlaceholderText = "Tìm kiếm nhanh theo Sân, SĐT, Mã đặt chỗ...",
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(38, factor),
            Margin = new Padding(0, 0, 0, DpiHelper.Scale(16, factor))
        };
        txtSearch.SearchSubmitted += (s, query) =>
        {
            MessageBox.Show($"Đã kích hoạt tìm kiếm: \"{query}\"", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        var emptyLabel = CreateSubHeaderLabel("2. Trạng thái danh sách rỗng (Empty State):", factor);
        var emptyControl = new EmptyStateControl
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(230, factor),
            Title = "Chưa có đơn đặt sân nào trong ca",
            Description = "Hiện tại ca này hoàn toàn trống. Bạn có thể chọn ô ca trên ma trận hoặc bấm nút bên dưới.",
            ActionButtonText = "Tạo đơn đặt sân mới",
            ShowActionButton = true
        };
        emptyControl.ActionClick += (s, e) =>
        {
            MessageBox.Show("Mở cửa sổ Đặt sân nhanh cho khách hàng!", "Đặt sân mới", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        rightCard.Controls.Add(emptyControl);
        rightCard.Controls.Add(emptyLabel);
        rightCard.Controls.Add(txtSearch);
        rightCard.Controls.Add(txtName);
        rightCard.Controls.Add(inputLabel);

        detailsTable.Controls.Add(leftCard, 0, 0);
        detailsTable.Controls.Add(rightCard, 1, 0);

        _controlsTabPanel.Controls.Add(detailsTable);
        _controlsTabPanel.Controls.Add(metricsTable);
    }

    private Panel CreateCardContainer(float factor, string title)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(DpiHelper.Scale(18, factor)),
            Margin = new Padding(DpiHelper.Scale(4, factor))
        };

        var titleLabel = new Label
        {
            Text = title,
            ForeColor = UITheme.TextSecondary,
            Font = new Font("Segoe UI Bold", Math.Max(8f, 9f * factor), FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(26, factor)
        };

        panel.Controls.Add(titleLabel);
        return panel;
    }

    private Label CreateSubHeaderLabel(string text, float factor)
    {
        return new Label
        {
            Text = text,
            ForeColor = UITheme.TextPrimary,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(28, factor),
            Margin = new Padding(0, DpiHelper.Scale(6, factor), 0, 0)
        };
    }

    // =========================================================================
    // TAB 2: COURT MATRIX GRID
    // =========================================================================

    private void BuildMatrixTab(float factor)
    {
        _matrixTabPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        // 1. Thanh chú giải & Bộ lọc (Top Filter Bar)
        int filterHeight = DpiHelper.Scale(48, factor);
        var filterBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = filterHeight,
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(16, factor), 0, DpiHelper.Scale(16, factor), 0)
        };

        var branchLabel = new Label
        {
            Text = "📍 Chi nhánh: SportChain Quận 7  |  📅 Hôm nay: 22/10/2026",
            ForeColor = UITheme.TextPrimary,
            Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold),
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = true
        };

        // Chú giải màu sắc trạng thái
        var legendFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, DpiHelper.Scale(8, factor), 0, 0)
        };

        var lg1 = new StatusBadge { Size = new Size(DpiHelper.Scale(85, factor), DpiHelper.Scale(26, factor)), ShowDot = true }; lg1.SetStatus("Available", "Trống");
        var lg2 = new StatusBadge { Size = new Size(DpiHelper.Scale(95, factor), DpiHelper.Scale(26, factor)), ShowDot = true }; lg2.SetStatus("PendingPayment", "Chờ cọc");
        var lg3 = new StatusBadge { Size = new Size(DpiHelper.Scale(95, factor), DpiHelper.Scale(26, factor)), ShowDot = true }; lg3.SetStatus("Confirmed", "Đã cọc");
        var lg4 = new StatusBadge { Size = new Size(DpiHelper.Scale(105, factor), DpiHelper.Scale(26, factor)), ShowDot = true }; lg4.SetStatus("InUse", "Đang đá");
        var lg5 = new StatusBadge { Size = new Size(DpiHelper.Scale(90, factor), DpiHelper.Scale(26, factor)), ShowDot = true }; lg5.SetStatus("Maintenance", "Bảo trì");

        legendFlow.Controls.AddRange(new Control[] { lg1, lg2, lg3, lg4, lg5 });

        filterBar.Controls.Add(branchLabel);
        filterBar.Controls.Add(legendFlow);

        // 2. Thanh thông báo tương tác bên dưới (Bottom Status Banner)
        int bannerHeight = DpiHelper.Scale(36, factor);
        var statusBanner = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = bannerHeight,
            BackColor = UITheme.NavySurface,
            Padding = new Padding(DpiHelper.Scale(16, factor), 0, DpiHelper.Scale(16, factor), 0)
        };

        _statusNotificationLabel = new Label
        {
            Text = "💡 Hãy rê chuột hoặc nhấp chọn một ô ca sân để xem thông tin chi tiết / đặt ca.",
            ForeColor = UITheme.TextLight,
            Font = new Font("Segoe UI", Math.Max(8f, 9f * factor), FontStyle.Regular),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        statusBanner.Controls.Add(_statusNotificationLabel);

        // 3. Bảng ma trận CourtMatrixGrid
        _courtGrid = new CourtMatrixGrid
        {
            Dock = DockStyle.Fill
        };

        _courtGrid.CellClicked += (s, e) =>
        {
            string cust = !string.IsNullOrWhiteSpace(e.Cell.CustomerName) ? $" | Khách: {e.Cell.CustomerName}" : "";
            string code = !string.IsNullOrWhiteSpace(e.Cell.BookingCode) ? $" | Mã: {e.Cell.BookingCode}" : "";
            _statusNotificationLabel.Text = $"👉 Đã chọn: {e.Row.CourtName} | Ca: {e.TimeSlot.DisplayName} | Trạng thái: {e.Cell.VisualStatus} | Giá: {e.Cell.Price:N0} đ{cust}{code}";
        };

        _courtGrid.CellDoubleClicked += (s, e) =>
        {
            string msg = $"🏟️ SÂN: {e.Row.CourtName} ({e.Row.CourtTypeName})\n" +
                         $"⏰ KHUNG GIỜ: {e.TimeSlot.DisplayName} (Ca {e.TimeSlot.SlotIndex})\n" +
                         $"🏷️ TRẠNG THÁI: {e.Cell.VisualStatus}\n" +
                         $"💰 ĐƠN GIÁ: {e.Cell.Price:N0} VNĐ\n" +
                         $"👤 KHÁCH HÀNG: {e.Cell.CustomerName ?? "Chưa có"}\n" +
                         $"📞 SỐ ĐIỆN THOẠI: {e.Cell.CustomerPhone ?? "Chưa có"}\n" +
                         $"🔖 MÃ ĐƠN: {e.Cell.BookingCode ?? "Chưa có"}";

            MessageBox.Show(msg, "Chi tiết ca sân", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        _matrixTabPanel.Controls.Add(_courtGrid);
        _matrixTabPanel.Controls.Add(filterBar);
        _matrixTabPanel.Controls.Add(statusBanner);
    }

    // =========================================================================
    // DỮ LIỆU GIẢ LẬP MA TRẬN SÂN (MOCK DATA GENERATION)
    // =========================================================================

    private void LoadMockData()
    {
        var response = new CourtGridResponseDto
        {
            BranchId = 1,
            BranchName = "SportChain - Chi nhánh Quận 7",
            SelectedDate = DateTime.Today
        };

        // 17 Khung giờ từ 06:00 đến 23:00 (mỗi ca 60 phút)
        for (int i = 6; i <= 22; i++)
        {
            var start = new TimeSpan(i, 0, 0);
            var end = new TimeSpan(i + 1, 0, 0);
            bool isPeak = (i >= 17 && i <= 21); // Giờ vàng: 17h - 22h

            response.TimeSlots.Add(new TimeSlotDto
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

            for (int s = 0; s < response.TimeSlots.Count; s++)
            {
                var slot = response.TimeSlots[s];
                decimal price = slot.IsPeakHour ? peakPrice : normalPrice;

                var cell = new CourtTimeSlotCellDto
                {
                    CourtId = row.CourtId,
                    TimeSlotId = slot.Id,
                    TimeSlotDisplayName = slot.DisplayName,
                    BookingDate = DateTime.Today,
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

            response.Rows.Add(row);
        }

        _courtGrid.LoadData(response);
    }
}
