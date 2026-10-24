using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Controls.Common;

namespace BTLtruc_quan.WinForms.Controls.Grids;

/// <summary>
/// Bảng lưới ma trận sân & ca trực quan thời gian thực (Owner-Drawn GDI+ DoubleBuffered).
/// Hỗ trợ cố định tiêu đề cột (TimeSlots) và tiêu đề dòng (Courts) khi cuộn mượt mà,
/// highlight ô khi hover, hiển thị tooltip chi tiết và hỗ trợ chọn nhiều ca liên tiếp.
/// </summary>
public class CourtMatrixGrid : ScrollableControl
{
    // Dữ liệu ma trận
    private CourtGridResponseDto? _data;

    // Kích thước cơ sở (chưa scale DPI)
    private const int BaseCourtHeaderWidth = 185;
    private const int BaseTimeSlotHeaderHeight = 56;
    private const int BaseCellWidth = 118;
    private const int BaseCellHeight = 68;

    // Trạng thái tương tác chuột
    private int _hoveredRow = -1;
    private int _hoveredCol = -1;
    private bool _isMouseDown = false;
    private int _dragStartRow = -1;
    private int _dragStartCol = -1;

    // Danh sách các ô đang được chọn
    private readonly HashSet<(int row, int col)> _selectedIndices = new();

    // Tooltip chi tiết
    private readonly ToolTip _toolTip;
    private string _lastTooltipText = string.Empty;

    // Events
    public event EventHandler<CourtCellClickedEventArgs>? CellClicked;
    public event EventHandler<CourtCellClickedEventArgs>? CellDoubleClicked;
    public event EventHandler<CourtSelectionChangedEventArgs>? SelectionChanged;

    public CourtMatrixGrid()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.Selectable, true);

        UpdateStyles();
        DoubleBuffered = true;
        AutoScroll = true;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9f, FontStyle.Regular);

        _toolTip = new ToolTip
        {
            InitialDelay = 300,
            ReshowDelay = 100,
            AutoPopDelay = 5000,
            IsBalloon = false
        };
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CourtGridResponseDto? Data
    {
        get => _data;
        set => LoadData(value);
    }

    [Browsable(false)]
    public IReadOnlyList<CourtTimeSlotCellDto> SelectedCells
    {
        get
        {
            var list = new List<CourtTimeSlotCellDto>();
            if (_data == null) return list;

            foreach (var (r, c) in _selectedIndices)
            {
                if (r >= 0 && r < _data.Rows.Count && c >= 0 && c < _data.Rows[r].Cells.Count)
                {
                    list.Add(_data.Rows[r].Cells[c]);
                }
            }
            return list;
        }
    }

    /// <summary>
    /// Nạp dữ liệu mới vào ma trận và tính toán lại kích thước cuộn
    /// </summary>
    public void LoadData(CourtGridResponseDto? data)
    {
        _data = data;
        _selectedIndices.Clear();
        _hoveredRow = -1;
        _hoveredCol = -1;
        RecalculateScrollBounds();
        Invalidate();
        OnSelectionChanged();
    }

    /// <summary>
    /// Làm mới lại dữ liệu của một ô cụ thể (khi nhận event SignalR cập nhật)
    /// </summary>
    public void UpdateCell(int courtId, int timeSlotId, Action<CourtTimeSlotCellDto> updateAction)
    {
        if (_data == null) return;

        for (int r = 0; r < _data.Rows.Count; r++)
        {
            if (_data.Rows[r].CourtId == courtId)
            {
                for (int c = 0; c < _data.Rows[r].Cells.Count; c++)
                {
                    if (_data.Rows[r].Cells[c].TimeSlotId == timeSlotId)
                    {
                        updateAction(_data.Rows[r].Cells[c]);
                        Invalidate();
                        return;
                    }
                }
            }
        }
    }

    private void RecalculateScrollBounds()
    {
        if (_data == null || _data.Rows.Count == 0 || _data.TimeSlots.Count == 0)
        {
            AutoScrollMinSize = Size.Empty;
            return;
        }

        float factor = DpiHelper.GetScaleFactor(this);
        int courtHeaderW = DpiHelper.Scale(BaseCourtHeaderWidth, factor);
        int slotHeaderH = DpiHelper.Scale(BaseTimeSlotHeaderHeight, factor);
        int cellW = DpiHelper.Scale(BaseCellWidth, factor);
        int cellH = DpiHelper.Scale(BaseCellHeight, factor);

        int totalWidth = courtHeaderW + (_data.TimeSlots.Count * cellW);
        int totalHeight = slotHeaderH + (_data.Rows.Count * cellH);

        AutoScrollMinSize = new Size(totalWidth, totalHeight);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        RecalculateScrollBounds();
        Invalidate();
    }

    protected override void OnScroll(ScrollEventArgs se)
    {
        base.OnScroll(se);
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        Invalidate();
    }

    // =========================================================================
    // VẼ ĐỒ HỌA GDI+ (PAINTING WITH FROZEN HEADERS)
    // =========================================================================

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        GraphicsHelper.ApplyHighQuality(g);

        if (_data == null || _data.Rows.Count == 0 || _data.TimeSlots.Count == 0)
        {
            DrawEmptyGridState(g);
            return;
        }

        float factor = DpiHelper.GetScaleFactor(this);
        int courtHeaderW = DpiHelper.Scale(BaseCourtHeaderWidth, factor);
        int slotHeaderH = DpiHelper.Scale(BaseTimeSlotHeaderHeight, factor);
        int cellW = DpiHelper.Scale(BaseCellWidth, factor);
        int cellH = DpiHelper.Scale(BaseCellHeight, factor);

        int scrollX = -AutoScrollPosition.X;
        int scrollY = -AutoScrollPosition.Y;

        int numRows = _data.Rows.Count;
        int numCols = _data.TimeSlots.Count;

        // Vùng hiển thị của phần nội dung ô ca (Grid Body)
        var bodyClipRect = new Rectangle(
            courtHeaderW,
            slotHeaderH,
            Math.Max(0, ClientRectangle.Width - courtHeaderW),
            Math.Max(0, ClientRectangle.Height - slotHeaderH)
        );

        // -------------------------------------------------------------
        // 1. VẼ NỘI DUNG CÁC Ô CA SÂN (CELLS) - Cắt theo bodyClipRect
        // -------------------------------------------------------------
        var originalClip = g.Clip;
        g.SetClip(bodyClipRect);

        for (int r = 0; r < numRows; r++)
        {
            int cellY = slotHeaderH + (r * cellH) - scrollY;

            // Kiểm tra ô có nằm trong tầm mắt theo trục Y không
            if (cellY + cellH < slotHeaderH || cellY > ClientRectangle.Height)
                continue;

            var rowData = _data.Rows[r];

            for (int c = 0; c < numCols; c++)
            {
                int cellX = courtHeaderW + (c * cellW) - scrollX;

                // Kiểm tra ô có nằm trong tầm mắt theo trục X không
                if (cellX + cellW < courtHeaderW || cellX > ClientRectangle.Width)
                    continue;

                CourtTimeSlotCellDto? cellData = c < rowData.Cells.Count ? rowData.Cells[c] : null;
                var cellRect = new Rectangle(cellX, cellY, cellW, cellH);

                bool isHovered = (r == _hoveredRow && c == _hoveredCol);
                bool isSelected = _selectedIndices.Contains((r, c));

                DrawCourtCell(g, cellRect, cellData, isHovered, isSelected, factor);
            }
        }

        // -------------------------------------------------------------
        // 2. VẼ DÒNG TIÊU ĐỀ KHUNG GIỜ CỐ ĐỊNH Ở TRÊN (COLUMN HEADERS)
        // -------------------------------------------------------------
        var topHeaderClipRect = new Rectangle(
            courtHeaderW,
            0,
            Math.Max(0, ClientRectangle.Width - courtHeaderW),
            slotHeaderH
        );
        g.SetClip(topHeaderClipRect);

        for (int c = 0; c < numCols; c++)
        {
            int colX = courtHeaderW + (c * cellW) - scrollX;
            if (colX + cellW < courtHeaderW || colX > ClientRectangle.Width)
                continue;

            var timeSlot = _data.TimeSlots[c];
            var colRect = new Rectangle(colX, 0, cellW, slotHeaderH);

            DrawTimeSlotHeaderCell(g, colRect, timeSlot, c == _hoveredCol, factor);
        }

        // -------------------------------------------------------------
        // 3. VẼ CỘT TIÊU ĐỀ SÂN CỐ ĐỊNH Ở BÊN TRÁI (ROW HEADERS)
        // -------------------------------------------------------------
        var leftHeaderClipRect = new Rectangle(
            0,
            slotHeaderH,
            courtHeaderW,
            Math.Max(0, ClientRectangle.Height - slotHeaderH)
        );
        g.SetClip(leftHeaderClipRect);

        for (int r = 0; r < numRows; r++)
        {
            int rowY = slotHeaderH + (r * cellH) - scrollY;
            if (rowY + cellH < slotHeaderH || rowY > ClientRectangle.Height)
                continue;

            var rowData = _data.Rows[r];
            var rowRect = new Rectangle(0, rowY, courtHeaderW, cellH);

            DrawCourtRowHeaderCell(g, rowRect, rowData, r == _hoveredRow, factor);
        }

        // -------------------------------------------------------------
        // 4. VẼ GÓC TRÊN CÙNG BÊN TRÁI (FROZEN TOP-LEFT CORNER)
        // -------------------------------------------------------------
        g.SetClip(new Rectangle(0, 0, courtHeaderW, slotHeaderH));
        DrawTopLeftCorner(g, new Rectangle(0, 0, courtHeaderW, slotHeaderH), factor);

        // Khôi phục vùng Clip gốc
        g.Clip = originalClip;

        // -------------------------------------------------------------
        // 5. VẼ CÁC ĐƯỜNG PHÂN CÁCH TIÊU ĐỀ VÀ BÓNG ĐỔ MỎNG
        // -------------------------------------------------------------
        using (var separatorPen = new Pen(UITheme.BorderColor, 1.5f))
        {
            // Đường phân cách ngang dưới header giờ
            g.DrawLine(separatorPen, 0, slotHeaderH - 1, ClientRectangle.Width, slotHeaderH - 1);
            // Đường phân cách dọc bên phải header sân
            g.DrawLine(separatorPen, courtHeaderW - 1, 0, courtHeaderW - 1, ClientRectangle.Height);
        }
    }

    /// <summary>
    /// Vẽ ô ca sân đơn lẻ với bảng màu SportChain theo trạng thái VisualStatus
    /// </summary>
    private void DrawCourtCell(Graphics g, Rectangle bounds, CourtTimeSlotCellDto? cell, bool isHovered, bool isSelected, float factor)
    {
        // Margin nhỏ 2px quanh ô để tạo cảm giác dạng thẻ phẳng rời rạc
        int margin = Math.Max(1, DpiHelper.Scale(2, factor));
        var innerRect = new Rectangle(
            bounds.X + margin,
            bounds.Y + margin,
            bounds.Width - (margin * 2),
            bounds.Height - (margin * 2)
        );

        int radius = DpiHelper.ScaleRadius(6, factor);

        if (cell == null)
        {
            using var emptyBrush = new SolidBrush(ColorTranslator.FromHtml("#F8FAFC"));
            GraphicsHelper.FillRoundedRectangle(g, emptyBrush, innerRect, radius);
            return;
        }

        string status = cell.VisualStatus ?? "Available";
        Color statusColor = UITheme.GetStatusColor(status);
        Color lightBgColor = UITheme.GetStatusLightColor(status);

        // 1. Tô màu nền ô
        using (var bgBrush = new SolidBrush(lightBgColor))
        {
            GraphicsHelper.FillRoundedRectangle(g, bgBrush, innerRect, radius);
        }

        // 2. Vẽ viền ô (Nổi bật khi Hover hoặc Selected)
        if (isSelected)
        {
            using var selectPen = new Pen(UITheme.PrimaryDark, 2.5f);
            GraphicsHelper.DrawRoundedRectangle(g, selectPen, innerRect, radius);
        }
        else if (isHovered)
        {
            using var hoverPen = new Pen(statusColor, 1.8f);
            GraphicsHelper.DrawRoundedRectangle(g, hoverPen, innerRect, radius);
        }
        else
        {
            // Viền mỏng dịu mắt
            using var normalPen = new Pen(Color.FromArgb(80, statusColor), 1f);
            GraphicsHelper.DrawRoundedRectangle(g, normalPen, innerRect, radius);
        }

        // 3. Thanh trạng thái màu nhỏ bên trái ô (Status Indicator Bar)
        int barWidth = DpiHelper.Scale(4, factor);
        var barRect = new Rectangle(innerRect.X + 1, innerRect.Y + 2, barWidth, innerRect.Height - 4);
        using (var barBrush = new SolidBrush(statusColor))
        {
            GraphicsHelper.FillRoundedRectangle(g, barBrush, barRect, 2);
        }

        // 4. Vẽ nội dung thông tin bên trong ô
        int contentX = innerRect.X + barWidth + DpiHelper.Scale(6, factor);
        int contentW = innerRect.Width - barWidth - DpiHelper.Scale(10, factor);

        switch (status.ToLowerInvariant())
        {
            case "available" or "trong":
                DrawAvailableCellContent(g, contentX, innerRect.Y + DpiHelper.Scale(6, factor), contentW, innerRect.Height, cell, factor);
                break;

            case "pendingpayment" or "chococ":
                DrawPendingCellContent(g, contentX, innerRect.Y + DpiHelper.Scale(4, factor), contentW, innerRect.Height, cell, factor);
                break;

            case "confirmed" or "daxacnhan":
                DrawConfirmedCellContent(g, contentX, innerRect.Y + DpiHelper.Scale(4, factor), contentW, innerRect.Height, cell, factor);
                break;

            case "inuse" or "dangthidau":
                DrawInUseCellContent(g, contentX, innerRect.Y + DpiHelper.Scale(4, factor), contentW, innerRect.Height, cell, factor);
                break;

            case "maintenance" or "baotri":
                DrawMaintenanceCellContent(g, contentX, innerRect.Y + DpiHelper.Scale(14, factor), contentW, innerRect.Height, factor);
                break;

            default:
                DrawAvailableCellContent(g, contentX, innerRect.Y + DpiHelper.Scale(6, factor), contentW, innerRect.Height, cell, factor);
                break;
        }
    }

    private void DrawAvailableCellContent(Graphics g, int x, int y, int w, int h, CourtTimeSlotCellDto cell, float factor)
    {
        // Chữ "Trống" hoặc "+ Đặt sân"
        using var statusFont = new Font("Segoe UI Semibold", Math.Max(7.5f, 8.5f * factor), FontStyle.Bold);
        using var statusBrush = new SolidBrush(UITheme.StatusAvailable);
        g.DrawString("Trống", statusFont, statusBrush, x, y);

        // Giá tiền ở góc dưới
        string priceText = cell.Price > 0 ? $"{cell.Price:N0}đ" : "Miễn phí";
        using var priceFont = new Font("Segoe UI", Math.Max(7f, 7.8f * factor), FontStyle.Regular);
        using var priceBrush = new SolidBrush(UITheme.TextSecondary);
        int priceY = y + DpiHelper.Scale(26, factor);
        g.DrawString(priceText, priceFont, priceBrush, x, priceY);
    }

    private void DrawPendingCellContent(Graphics g, int x, int y, int w, int h, CourtTimeSlotCellDto cell, float factor)
    {
        using var labelFont = new Font("Segoe UI Bold", Math.Max(7.5f, 8f * factor), FontStyle.Bold);
        using var labelBrush = new SolidBrush(UITheme.StatusPendingPayment);
        g.DrawString("CHỜ CỌC", labelFont, labelBrush, x, y);

        string bookingCode = !string.IsNullOrEmpty(cell.BookingCode) ? cell.BookingCode : "Đang giữ ca";
        using var codeFont = new Font("Segoe UI", Math.Max(7f, 7.8f * factor), FontStyle.Regular);
        using var codeBrush = new SolidBrush(UITheme.TextPrimary);
        g.DrawString(bookingCode, codeFont, codeBrush, new RectangleF(x, y + DpiHelper.Scale(16, factor), w, DpiHelper.Scale(16, factor)));

        // Giá tiền
        string priceText = $"{cell.Price:N0}đ";
        using var priceFont = new Font("Segoe UI", Math.Max(6.8f, 7.5f * factor), FontStyle.Regular);
        using var priceBrush = new SolidBrush(UITheme.TextMuted);
        g.DrawString(priceText, priceFont, priceBrush, x, y + DpiHelper.Scale(32, factor));
    }

    private void DrawConfirmedCellContent(Graphics g, int x, int y, int w, int h, CourtTimeSlotCellDto cell, float factor)
    {
        string custName = !string.IsNullOrWhiteSpace(cell.CustomerName) ? cell.CustomerName : "Khách đã đặt";
        using var nameFont = new Font("Segoe UI Semibold", Math.Max(7.5f, 8.5f * factor), FontStyle.Bold);
        using var nameBrush = new SolidBrush(UITheme.TextPrimary);
        g.DrawString(custName, nameFont, nameBrush, new RectangleF(x, y, w, DpiHelper.Scale(18, factor)));

        string phone = !string.IsNullOrWhiteSpace(cell.CustomerPhone) ? cell.CustomerPhone : "Đã đặt cọc";
        using var phoneFont = new Font("Segoe UI", Math.Max(6.8f, 7.5f * factor), FontStyle.Regular);
        using var phoneBrush = new SolidBrush(UITheme.StatusConfirmed);
        g.DrawString(phone, phoneFont, phoneBrush, x, y + DpiHelper.Scale(18, factor));

        // Giá tiền
        string priceText = $"{cell.Price:N0}đ";
        using var priceFont = new Font("Segoe UI", Math.Max(6.8f, 7.5f * factor), FontStyle.Regular);
        using var priceBrush = new SolidBrush(UITheme.TextMuted);
        g.DrawString(priceText, priceFont, priceBrush, x, y + DpiHelper.Scale(33, factor));
    }

    private void DrawInUseCellContent(Graphics g, int x, int y, int w, int h, CourtTimeSlotCellDto cell, float factor)
    {
        using var badgeFont = new Font("Segoe UI Bold", Math.Max(7.5f, 8f * factor), FontStyle.Bold);
        using var badgeBrush = new SolidBrush(UITheme.StatusInUse);
        g.DrawString("ĐANG THI ĐẤU", badgeFont, badgeBrush, x, y);

        string custName = !string.IsNullOrWhiteSpace(cell.CustomerName) ? cell.CustomerName : "Đang sử dụng";
        using var nameFont = new Font("Segoe UI", Math.Max(7f, 7.8f * factor), FontStyle.Regular);
        using var nameBrush = new SolidBrush(UITheme.TextPrimary);
        g.DrawString(custName, nameFont, nameBrush, new RectangleF(x, y + DpiHelper.Scale(16, factor), w, DpiHelper.Scale(16, factor)));

        // Giá tiền
        string priceText = $"{cell.Price:N0}đ";
        using var priceFont = new Font("Segoe UI", Math.Max(6.8f, 7.5f * factor), FontStyle.Regular);
        using var priceBrush = new SolidBrush(UITheme.TextMuted);
        g.DrawString(priceText, priceFont, priceBrush, x, y + DpiHelper.Scale(32, factor));
    }

    private void DrawMaintenanceCellContent(Graphics g, int x, int y, int w, int h, float factor)
    {
        using var font = new Font("Segoe UI Semibold", Math.Max(7.5f, 8.5f * factor), FontStyle.Bold);
        using var brush = new SolidBrush(UITheme.StatusMaintenance);
        g.DrawString("BẢO TRÌ", font, brush, x, y);

        using var subFont = new Font("Segoe UI", Math.Max(6.8f, 7.5f * factor), FontStyle.Regular);
        using var subBrush = new SolidBrush(UITheme.TextMuted);
        g.DrawString("Tạm khóa", subFont, subBrush, x, y + DpiHelper.Scale(16, factor));
    }

    /// <summary>
    /// Vẽ ô tiêu đề cột khung giờ (TimeSlot Header)
    /// </summary>
    private void DrawTimeSlotHeaderCell(Graphics g, Rectangle bounds, TimeSlotDto slot, bool isHovered, float factor)
    {
        // Nền Header giờ
        Color bgColor = isHovered ? UITheme.HoverBackground : ColorTranslator.FromHtml("#F8FAFC");
        using (var brush = new SolidBrush(bgColor))
        {
            g.FillRectangle(brush, bounds);
        }

        // Viền phải và viền dưới
        using (var pen = new Pen(UITheme.BorderColor, 1f))
        {
            g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        // Tên ca (Ca 1, Ca 2...) hoặc thời gian
        string slotTitle = $"Ca {slot.SlotIndex}";
        using (var titleFont = new Font("Segoe UI", Math.Max(7f, 7.8f * factor), FontStyle.Regular))
        using (var titleBrush = new SolidBrush(UITheme.TextSecondary))
        {
            int titleY = bounds.Top + DpiHelper.Scale(6, factor);
            g.DrawString(slotTitle, titleFont, titleBrush, bounds.Left + DpiHelper.Scale(8, factor), titleY);
        }

        // Giờ bắt đầu - Giờ kết thúc
        string timeRange = slot.DisplayName;
        using (var timeFont = new Font("Segoe UI Semibold", Math.Max(7.5f, 8.5f * factor), FontStyle.Bold))
        using (var timeBrush = new SolidBrush(slot.IsPeakHour ? UITheme.StatusConfirmed : UITheme.TextPrimary))
        {
            int timeY = bounds.Top + DpiHelper.Scale(26, factor);
            g.DrawString(timeRange, timeFont, timeBrush, bounds.Left + DpiHelper.Scale(8, factor), timeY);
        }

        // Nếu là giờ vàng (Peak hour), vẽ chấm cam nhỏ nhận diện
        if (slot.IsPeakHour)
        {
            int dotSize = DpiHelper.Scale(6, factor);
            int dotX = bounds.Right - dotSize - DpiHelper.Scale(8, factor);
            int dotY = bounds.Top + DpiHelper.Scale(8, factor);
            using var peakBrush = new SolidBrush(UITheme.StatusConfirmed);
            g.FillEllipse(peakBrush, dotX, dotY, dotSize, dotSize);
        }
    }

    /// <summary>
    /// Vẽ ô tiêu đề dòng sân (Court Header) cố định bên trái
    /// </summary>
    private void DrawCourtRowHeaderCell(Graphics g, Rectangle bounds, CourtGridRowDto row, bool isHovered, float factor)
    {
        Color bgColor = isHovered ? UITheme.HoverBackground : ColorTranslator.FromHtml("#F8FAFC");
        using (var brush = new SolidBrush(bgColor))
        {
            g.FillRectangle(brush, bounds);
        }

        // Viền dưới và viền phải
        using (var pen = new Pen(UITheme.BorderColor, 1f))
        {
            g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
        }

        int paddingX = DpiHelper.Scale(12, factor);
        int topY = bounds.Top + DpiHelper.Scale(8, factor);

        // Tên sân nổi bật (CourtName)
        using (var nameFont = new Font("Segoe UI Semibold", Math.Max(8.5f, 9.5f * factor), FontStyle.Bold))
        using (var nameBrush = new SolidBrush(UITheme.TextPrimary))
        {
            g.DrawString(row.CourtName, nameFont, nameBrush, bounds.Left + paddingX, topY);
        }

        // Mã sân & Loại sân
        string courtSub = $"{row.CourtCode} • {row.CourtTypeName}";
        using (var subFont = new Font("Segoe UI", Math.Max(7f, 7.8f * factor), FontStyle.Regular))
        using (var subBrush = new SolidBrush(UITheme.TextSecondary))
        {
            g.DrawString(courtSub, subFont, subBrush, bounds.Left + paddingX, topY + DpiHelper.Scale(22, factor));
        }

        // Điểm nhấn viền màu xanh bên trái nếu đang hover
        if (isHovered)
        {
            using var activeBarBrush = new SolidBrush(UITheme.Primary);
            g.FillRectangle(activeBarBrush, bounds.Left, bounds.Top, DpiHelper.Scale(3, factor), bounds.Height);
        }
    }

    /// <summary>
    /// Vẽ ô góc trên cùng bên trái (Top-Left Frozen Corner)
    /// </summary>
    private void DrawTopLeftCorner(Graphics g, Rectangle bounds, float factor)
    {
        using (var bgBrush = new SolidBrush(ColorTranslator.FromHtml("#F1F5F9")))
        {
            g.FillRectangle(bgBrush, bounds);
        }

        // Đường kẻ chéo ngăn cách Sân / Khung giờ
        using (var linePen = new Pen(UITheme.BorderColor, 1f))
        {
            g.DrawLine(linePen, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
            g.DrawLine(linePen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            g.DrawLine(linePen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        // Chữ "KHUNG GIỜ" ở góc trên bên phải & "SÂN THỂ THAO" ở góc dưới bên trái
        using (var font = new Font("Segoe UI Bold", Math.Max(7f, 7.8f * factor), FontStyle.Bold))
        using (var brush = new SolidBrush(UITheme.TextSecondary))
        {
            var kgSize = g.MeasureString("KHUNG GIỜ", font);
            float kgX = bounds.Right - kgSize.Width - DpiHelper.Scale(8, factor);
            float kgY = bounds.Top + DpiHelper.Scale(7, factor);
            g.DrawString("KHUNG GIỜ", font, brush, kgX, kgY);

            var sttSize = g.MeasureString("SÂN THỂ THAO", font);
            float sttX = bounds.Left + DpiHelper.Scale(10, factor);
            float sttY = bounds.Bottom - sttSize.Height - DpiHelper.Scale(7, factor);
            g.DrawString("SÂN THỂ THAO", font, brush, sttX, sttY);
        }
    }

    /// <summary>
    /// Vẽ trạng thái trống khi chưa có dữ liệu lưới
    /// </summary>
    private void DrawEmptyGridState(Graphics g)
    {
        using var font = new Font("Segoe UI", 11f, FontStyle.Regular);
        using var brush = new SolidBrush(UITheme.TextSecondary);
        string msg = "Chưa có dữ liệu lịch sân. Vui lòng chọn Chi nhánh và Ngày để tải lịch.";

        var size = g.MeasureString(msg, font);
        float x = Math.Max(10, (Width - size.Width) / 2);
        float y = Math.Max(10, (Height - size.Height) / 2);

        g.DrawString(msg, font, brush, x, y);
    }

    // =========================================================================
    // XỬ LÝ SỰ KIỆN CHUỘT (HIT TESTING, HOVER, SELECTION & TOOLTIP)
    // =========================================================================

    private bool HitTestCell(int mouseX, int mouseY, out int rowIndex, out int colIndex)
    {
        rowIndex = -1;
        colIndex = -1;

        if (_data == null || _data.Rows.Count == 0 || _data.TimeSlots.Count == 0)
            return false;

        float factor = DpiHelper.GetScaleFactor(this);
        int courtHeaderW = DpiHelper.Scale(BaseCourtHeaderWidth, factor);
        int slotHeaderH = DpiHelper.Scale(BaseTimeSlotHeaderHeight, factor);
        int cellW = DpiHelper.Scale(BaseCellWidth, factor);
        int cellH = DpiHelper.Scale(BaseCellHeight, factor);

        // Bỏ qua nếu click vào vùng header
        if (mouseX < courtHeaderW || mouseY < slotHeaderH)
            return false;

        int scrollX = -AutoScrollPosition.X;
        int scrollY = -AutoScrollPosition.Y;

        int relativeX = mouseX - courtHeaderW + scrollX;
        int relativeY = mouseY - slotHeaderH + scrollY;

        colIndex = relativeX / cellW;
        rowIndex = relativeY / cellH;

        return (rowIndex >= 0 && rowIndex < _data.Rows.Count &&
                colIndex >= 0 && colIndex < _data.TimeSlots.Count);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (HitTestCell(e.X, e.Y, out int r, out int c))
        {
            Cursor = Cursors.Hand;

            // Xử lý kéo chọn nhiều ca liên tiếp trên cùng 1 sân
            if (_isMouseDown && _dragStartRow == r)
            {
                int minCol = Math.Min(_dragStartCol, c);
                int maxCol = Math.Max(_dragStartCol, c);

                _selectedIndices.Clear();
                for (int col = minCol; col <= maxCol; col++)
                {
                    _selectedIndices.Add((r, col));
                }
                Invalidate();
                OnSelectionChanged();
            }

            if (_hoveredRow != r || _hoveredCol != c)
            {
                _hoveredRow = r;
                _hoveredCol = c;
                Invalidate();
                UpdateTooltipForCell(r, c);
            }
        }
        else
        {
            Cursor = Cursors.Default;
            if (_hoveredRow != -1 || _hoveredCol != -1)
            {
                _hoveredRow = -1;
                _hoveredCol = -1;
                _toolTip.Hide(this);
                _lastTooltipText = string.Empty;
                Invalidate();
            }
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoveredRow = -1;
        _hoveredCol = -1;
        _toolTip.Hide(this);
        _lastTooltipText = string.Empty;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();

        if (HitTestCell(e.X, e.Y, out int r, out int c))
        {
            _isMouseDown = true;
            _dragStartRow = r;
            _dragStartCol = c;

            // Nếu không bấm giữ phím Ctrl thì xóa vùng chọn cũ
            if ((ModifierKeys & Keys.Control) != Keys.Control)
            {
                _selectedIndices.Clear();
            }

            _selectedIndices.Add((r, c));
            Invalidate();
            OnSelectionChanged();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _isMouseDown = false;

        if (HitTestCell(e.X, e.Y, out int r, out int c))
        {
            if (_data != null && r < _data.Rows.Count && c < _data.Rows[r].Cells.Count)
            {
                var cell = _data.Rows[r].Cells[c];
                var row = _data.Rows[r];
                var timeSlot = _data.TimeSlots[c];
                CellClicked?.Invoke(this, new CourtCellClickedEventArgs(cell, row, timeSlot, e.Button));
            }
        }
    }

    protected override void OnDoubleClick(EventArgs e)
    {
        base.OnDoubleClick(e);
        var mousePos = PointToClient(Cursor.Position);

        if (HitTestCell(mousePos.X, mousePos.Y, out int r, out int c))
        {
            if (_data != null && r < _data.Rows.Count && c < _data.Rows[r].Cells.Count)
            {
                var cell = _data.Rows[r].Cells[c];
                var row = _data.Rows[r];
                var timeSlot = _data.TimeSlots[c];
                CellDoubleClicked?.Invoke(this, new CourtCellClickedEventArgs(cell, row, timeSlot, MouseButtons.Left));
            }
        }
    }

    private void OnSelectionChanged()
    {
        SelectionChanged?.Invoke(this, new CourtSelectionChangedEventArgs(SelectedCells));
    }

    /// <summary>
    /// Hiển thị tooltip chi tiết khi rê chuột vào ô ca sân
    /// </summary>
    private void UpdateTooltipForCell(int r, int c)
    {
        if (_data == null || r >= _data.Rows.Count || c >= _data.Rows[r].Cells.Count)
            return;

        var cell = _data.Rows[r].Cells[c];
        var row = _data.Rows[r];
        var slot = _data.TimeSlots[c];

        string statusDesc = cell.VisualStatus switch
        {
            "Available" => "Trống (Có thể đặt)",
            "PendingPayment" => "Chờ đặt cọc",
            "Confirmed" => "Đã xác nhận đặt",
            "InUse" => "Đang thi đấu",
            "Maintenance" => "Tạm khóa bảo trì",
            _ => cell.VisualStatus
        };

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"🏟️ Sân: {row.CourtName} ({row.CourtTypeName})");
        sb.AppendLine($"⏰ Khung giờ: {slot.DisplayName} (Ca {slot.SlotIndex})");
        sb.AppendLine($"🏷️ Trạng thái: {statusDesc}");
        sb.AppendLine($"💰 Đơn giá: {cell.Price:N0} VNĐ");

        if (!string.IsNullOrWhiteSpace(cell.BookingCode))
            sb.AppendLine($"🔖 Mã đơn: {cell.BookingCode}");
        if (!string.IsNullOrWhiteSpace(cell.CustomerName))
            sb.AppendLine($"👤 Khách hàng: {cell.CustomerName}");
        if (!string.IsNullOrWhiteSpace(cell.CustomerPhone))
            sb.AppendLine($"📞 Điện thoại: {cell.CustomerPhone}");

        string newTooltip = sb.ToString().TrimEnd();
        if (_lastTooltipText != newTooltip)
        {
            _lastTooltipText = newTooltip;
            _toolTip.SetToolTip(this, newTooltip);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _toolTip.Dispose();
        }
        base.Dispose(disposing);
    }
}
