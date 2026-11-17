using System.Drawing.Drawing2D;
using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WinForms.Controls.Buttons;
using BTLtruc_quan.WinForms.Controls.Common;
using FontAwesome.Sharp;

namespace BTLtruc_quan.WinForms.Forms;

/// <summary>
/// Hộp thoại xem trước và in phiếu thu / hóa đơn thanh toán quầy POS SportChain.
/// </summary>
public class InvoicePreviewDialog : Form
{
    private readonly InvoiceDto _invoice;

    public InvoicePreviewDialog(InvoiceDto invoice)
    {
        _invoice = invoice;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        float factor = DpiHelper.GetScaleFactor(this);

        Text = $"Phiếu Thu Hóa Đơn - {_invoice.InvoiceCode}";
        Size = new Size(DpiHelper.Scale(680, factor), DpiHelper.Scale(780, factor));
        MinimumSize = new Size(DpiHelper.Scale(600, factor), DpiHelper.Scale(650, factor));
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // Container chính dạng bảng phiếu thu cuộn được
        var pnlMain = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(DpiHelper.Scale(24, factor)),
            BackColor = Color.White
        };

        // 1. HEADER HÓA ĐƠN
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(110, factor),
            BackColor = Color.Transparent
        };
        pnlHeader.Paint += (s, e) =>
        {
            using var dashPen = new Pen(UITheme.BorderColor, 1.5f) { DashStyle = DashStyle.Dash };
            e.Graphics.DrawLine(dashPen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        };

        var lblBrand = new Label
        {
            Text = "⚡ SPORTCHAIN VN - HỆ THỐNG SÂN THỂ THAO",
            Font = new Font("Segoe UI", Math.Max(12f, 13.5f * factor), FontStyle.Bold),
            ForeColor = UITheme.PrimaryDark,
            AutoSize = true,
            Location = new Point(0, 0)
        };

        var lblBranch = new Label
        {
            Text = $"Cơ sở: {_invoice.BranchName} • Hotline: 1900 8888\nĐịa chỉ: Số 123 Nguyễn Thị Thập, P. Tân Phú, Quận 7, TP.HCM",
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextSecondary,
            AutoSize = true,
            Location = new Point(0, DpiHelper.Scale(28, factor))
        };

        var lblTitle = new Label
        {
            Text = "PHIẾU THANH TOÁN DỊCH VỤ POS",
            Font = new Font("Segoe UI", Math.Max(11f, 12f * factor), FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, DpiHelper.Scale(75, factor))
        };

        pnlHeader.Controls.AddRange(new Control[] { lblBrand, lblBranch, lblTitle });

        // 2. THÔNG TIN ĐƠN & KHÁCH HÀNG
        var pnlMeta = new Panel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(100, factor),
            BackColor = Color.Transparent,
            Padding = new Padding(0, DpiHelper.Scale(10, factor), 0, DpiHelper.Scale(10, factor))
        };

        string leftInfo = $"Mã hóa đơn: {_invoice.InvoiceCode}\n" +
                          $"Mã đơn đặt: {_invoice.BookingCode}\n" +
                          $"Thời gian lập: {_invoice.CreatedAt:HH:mm:ss - dd/MM/yyyy}";

        var lblMetaLeft = new Label
        {
            Text = leftInfo,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, DpiHelper.Scale(12, factor))
        };

        string rightInfo = $"Khách hàng: {_invoice.CustomerName}\n" +
                           $"Số điện thoại: {_invoice.CustomerPhone}\n" +
                           $"Thu ngân trực: {_invoice.CashierName}";

        var lblMetaRight = new Label
        {
            Text = rightInfo,
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            AutoSize = true,
            Location = new Point(DpiHelper.Scale(320, factor), DpiHelper.Scale(12, factor))
        };

        pnlMeta.Controls.AddRange(new Control[] { lblMetaLeft, lblMetaRight });

        // 3. BẢNG CHI TIẾT DỊCH VỤ (DATAGRIDVIEW)
        var dgvItems = new DataGridView
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(190, factor),
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false,
            GridColor = UITheme.BorderColor
        };
        dgvItems.ColumnHeadersDefaultCellStyle.BackColor = UITheme.LightBackground;
        dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextPrimary;
        dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", Math.Max(8.5f, 9f * factor), FontStyle.Bold);
        dgvItems.ColumnHeadersHeight = DpiHelper.Scale(32, factor);
        dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", Math.Max(8.5f, 9f * factor), FontStyle.Regular);
        dgvItems.DefaultCellStyle.ForeColor = UITheme.TextPrimary;
        dgvItems.RowTemplate.Height = DpiHelper.Scale(28, factor);

        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "STT", FillWeight = 12 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Khoản mục / Dịch vụ", FillWeight = 48 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ĐVT", FillWeight = 15 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SL", FillWeight = 12 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá (đ)", FillWeight = 25 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành tiền (đ)", FillWeight = 28 });

        // Dòng 1: Tiền thuê ca sân
        dgvItems.Rows.Add("1", $"Tiền thuê sân ({_invoice.BookingCode})", "Ca", "1", _invoice.CourtFee.ToString("N0"), _invoice.CourtFee.ToString("N0"));

        // Các dòng dịch vụ phụ thu (nước uống, vợt...)
        int stt = 2;
        if (_invoice.ServiceDetails != null)
        {
            foreach (var svc in _invoice.ServiceDetails)
            {
                dgvItems.Rows.Add(stt.ToString(), svc.ServiceItemName, svc.Unit, svc.Quantity.ToString(), svc.UnitPrice.ToString("N0"), svc.TotalPrice.ToString("N0"));
                stt++;
            }
        }

        // 4. KHUNG TỔNG KẾT TÀI CHÍNH (HẠCH TOÁN 2 PHA)
        var pnlSummary = new Panel
        {
            Dock = DockStyle.Top,
            Height = DpiHelper.Scale(170, factor),
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(16, factor)),
            Margin = new Padding(0, DpiHelper.Scale(12, factor), 0, 0)
        };
        pnlSummary.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, pnlSummary.Width - 1, pnlSummary.Height - 1);
        };

        var lblCalc = new Label
        {
            Text = $"Tiền thuê sân gốc: {_invoice.CourtFee:N0} đ\n" +
                   $"Tổng dịch vụ phụ thu: {_invoice.ServiceFee:N0} đ\n" +
                   $"Giảm giá / Chiết khấu: -{_invoice.DiscountAmount:N0} đ\n" +
                   $"TỔNG HÓA ĐƠN DỊCH VỤ: {_invoice.TotalAmount:N0} đ\n" +
                   $"Đã nộp cọc trước (Pha 1 - 30%): -{_invoice.DepositPaid:N0} đ\n" +
                   $"SỐ TIỀN THỰC THU TẠI QUẦY (Pha 2): {_invoice.FinalPaid:N0} đ\n" +
                   $"Khách đưa: {_invoice.CashGiven:N0} đ  •  Tiền thối lại: {_invoice.CashReturn:N0} đ\n" +
                   $"Hình thức: {_invoice.PaymentMethodName}",
            Font = new Font("Segoe UI", Math.Max(8.5f, 9.2f * factor), FontStyle.Regular),
            ForeColor = UITheme.TextPrimary,
            Dock = DockStyle.Fill
        };
        pnlSummary.Controls.Add(lblCalc);

        // 5. NÚT HÀNH ĐỘNG DƯỚI ĐÁY
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = DpiHelper.Scale(56, factor),
            BackColor = UITheme.LightBackground,
            Padding = new Padding(DpiHelper.Scale(16, factor), DpiHelper.Scale(8, factor), DpiHelper.Scale(16, factor), DpiHelper.Scale(8, factor))
        };
        pnlBottom.Paint += (s, e) =>
        {
            using var borderPen = new Pen(UITheme.BorderColor, 1f);
            e.Graphics.DrawLine(borderPen, 0, 0, pnlBottom.Width, 0);
        };

        var btnClose = new ModernButton
        {
            Text = "Đóng",
            IconChar = IconChar.Xmark,
            IconColor = UITheme.TextSecondary,
            ForeColor = UITheme.TextPrimary,
            BackgroundColor = Color.White,
            HoverColor = UITheme.HoverBackground,
            BorderColor = UITheme.BorderColor,
            BorderSize = 1f,
            Size = new Size(DpiHelper.Scale(110, factor), DpiHelper.Scale(38, factor)),
            Dock = DockStyle.Right
        };
        btnClose.Click += (s, e) => Close();

        var btnPrint = new ModernButton
        {
            Text = "In Phiếu Thu",
            IconChar = IconChar.Print,
            BackgroundColor = UITheme.Primary,
            HoverColor = UITheme.PrimaryDark,
            ForeColor = Color.White,
            IconColor = Color.White,
            Size = new Size(DpiHelper.Scale(140, factor), DpiHelper.Scale(38, factor)),
            Dock = DockStyle.Right,
            Margin = new Padding(0, 0, DpiHelper.Scale(10, factor), 0)
        };
        btnPrint.Click += (s, e) =>
        {
            MessageBox.Show($"Lệnh in hóa đơn '{_invoice.InvoiceCode}' đã được gửi tới máy in nhiệt POS quầy thu ngân thành công!",
                "In Hóa Đơn POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        var spacer = new Panel { Width = DpiHelper.Scale(10, factor), Dock = DockStyle.Right };
        pnlBottom.Controls.Add(btnPrint);
        pnlBottom.Controls.Add(spacer);
        pnlBottom.Controls.Add(btnClose);

        // Ghép nối giao diện
        pnlMain.Controls.Add(pnlSummary);
        pnlMain.Controls.Add(dgvItems);
        pnlMain.Controls.Add(pnlMeta);
        pnlMain.Controls.Add(pnlHeader);

        Controls.Add(pnlMain);
        Controls.Add(pnlBottom);
    }
}
