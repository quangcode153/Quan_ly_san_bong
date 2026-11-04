using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Data;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Payments;
using BTLtruc_quan.WebApi.Entities.Services;
using Microsoft.EntityFrameworkCore;

namespace BTLtruc_quan.WebApi.Services;

/// <summary>
/// Hiện thực hóa dịch vụ Thu ngân POS tại quầy (Check-in, Bán dịch vụ, Check-out & In hóa đơn)
/// Căn cứ: doc/specifications/BUSINESS_RULES_AND_STATE_MACHINE.md (QT04, QT07, QT18)
/// </summary>
public class PosService : IPosService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public PosService(AppDbContext context, IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<PosCheckInResponseDto> CheckInAsync(PosCheckInRequest request, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Slots)
                .ThenInclude(s => s.Court)
            .Include(b => b.Slots)
                .ThenInclude(s => s.Slot)
            .FirstOrDefaultAsync(b => b.Code == request.BookingCode || (b.Customer != null && b.Customer.Phone == request.BookingCode), cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy đơn đặt với mã hoặc SĐT '{request.BookingCode}'.");
        }

        if (booking.Status == BookingStatus.Holding && booking.HoldExpiresAt != default && booking.HoldExpiresAt < DateTime.UtcNow)
        {
            booking.Status = BookingStatus.Expired;
            await _context.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException($"Đơn đặt đã hết thời gian giữ chỗ (Hết hạn lúc {booking.HoldExpiresAt:HH:mm dd/MM/yyyy}), không thể check-in.");
        }

        if (booking.PlayDate != DateOnly.FromDateTime(DateTime.Today))
        {
            throw new InvalidOperationException($"Không thể check-in đơn đặt của ngày khác (Ngày đặt: {booking.PlayDate:dd/MM/yyyy}, Ngày hiện tại: {DateTime.Today:dd/MM/yyyy}).");
        }

        if (booking.Status == BookingStatus.Holding && booking.HoldExpiresAt != default && booking.HoldExpiresAt < DateTime.UtcNow)
        {
            booking.Status = BookingStatus.Expired;
            await _context.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException($"Đơn đặt đã hết thời gian giữ chỗ (Hết hạn lúc {booking.HoldExpiresAt:HH:mm dd/MM/yyyy}), không thể check-in.");
        }

        if (booking.PlayDate != DateOnly.FromDateTime(DateTime.Today))
        {
            throw new InvalidOperationException($"Không thể check-in đơn đặt của ngày khác (Ngày đặt: {booking.PlayDate:dd/MM/yyyy}, Ngày hiện tại: {DateTime.Today:dd/MM/yyyy}).");
        }

        if (booking.Status != BookingStatus.Confirmed && booking.Status != BookingStatus.Holding)
        {
            throw new InvalidOperationException($"Đơn đặt đang ở trạng thái '{booking.Status}', không thể thực hiện check-in.");
        }

        string oldStatus = booking.Status.ToString();
        booking.Status = BookingStatus.CheckedIn;
        booking.CheckedInAt = DateTime.UtcNow;
        booking.CheckedInByName = request.PerformerName;
        booking.CheckinMethod = CheckinMethod.Qr;
        if (!string.IsNullOrWhiteSpace(request.StaffNotes))
        {
            booking.Note = string.IsNullOrWhiteSpace(booking.Note) ? request.StaffNotes : $"{booking.Note} | {request.StaffNotes}";
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Ghi Audit Log tự động (QT18)
        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = request.PerformerName,
            BranchId = (int)booking.BranchId,
            Action = "CHECKIN",
            EntityName = "Booking",
            EntityId = booking.Id.ToString(),
            OldValue = oldStatus,
            NewValue = BookingStatus.CheckedIn.ToString(),
            Notes = $"Check-in thành công mã đơn {booking.Code}"
        }, cancellationToken);

        var firstSlot = booking.Slots.FirstOrDefault();
        string timeDisplay = firstSlot != null 
            ? $"{firstSlot.StartAt:HH:mm} - {firstSlot.EndAt:HH:mm}" 
            : $"{booking.StartAt:HH:mm} - {booking.EndAt:HH:mm}";

        return new PosCheckInResponseDto
        {
            BookingId = (int)booking.Id,
            BookingCode = booking.Code,
            CustomerName = booking.Customer?.FullName ?? "Khách vãng lai",
            CustomerPhone = booking.Customer?.Phone ?? string.Empty,
            CourtId = firstSlot != null ? (int)firstSlot.CourtId : 0,
            CourtName = firstSlot?.Court?.Name ?? "Sân thể thao",
            BookingDate = booking.PlayDate.ToDateTime(TimeOnly.MinValue),
            TimeSlotDisplayName = timeDisplay,
            TotalCourtFee = booking.SlotAmount,
            DepositPaid = booking.DepositAmount,
            Status = booking.Status,
            CheckInTime = booking.CheckedInAt.Value
        };
    }

    public async Task<BookingServiceDetailDto> AddServiceAsync(PosAddServiceRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Số lượng dịch vụ phải lớn hơn 0.");
        }

        var booking = await _context.Bookings.FindAsync(new object[] { (long)request.BookingId }, cancellationToken);
        if (booking == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy đơn đặt ID {request.BookingId}.");
        }

        var service = await _context.Services.FindAsync(new object[] { (long)request.ServiceItemId }, cancellationToken);
        if (service == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy dịch vụ/nước uống ID {request.ServiceItemId}.");
        }

        // Kiểm tra tồn kho
        if (service.StockQty.HasValue && service.StockQty.Value < request.Quantity)
        {
            throw new InvalidOperationException($"Số lượng tồn kho không đủ để đáp ứng yêu cầu (Còn: {service.StockQty.Value}, Yêu cầu: {request.Quantity}).");
        }

        // Trừ kho
        if (service.StockQty.HasValue)
        {
            service.StockQty -= request.Quantity;
        }

        decimal lineTotal = service.Price * request.Quantity;

        var bookingService = new BookingService
        {
            BookingId = booking.Id,
            ServiceId = service.Id,
            NameSnapshot = service.Name,
            Quantity = request.Quantity,
            UnitPriceSnapshot = service.Price,
            Amount = lineTotal,
            AddedWhen = ServiceAddedWhen.OnSite,
            AddedByName = request.PerformerName,
            AddedAt = DateTime.UtcNow
        };

        _context.BookingServices.Add(bookingService);

        // Cập nhật tổng tiền đơn đặt
        booking.ServiceAmount += lineTotal;
        booking.TotalAmount = booking.SlotAmount + booking.ServiceAmount - booking.DiscountAmount;

        await _context.SaveChangesAsync(cancellationToken);

        // Ghi Audit Log
        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = request.PerformerName,
            BranchId = (int)booking.BranchId,
            Action = "ADD_SERVICE",
            EntityName = "BookingService",
            EntityId = bookingService.Id.ToString(),
            OldValue = null,
            NewValue = $"Thêm {request.Quantity} {service.Unit} {service.Name} = {lineTotal:N0}đ",
            Notes = $"Thêm dịch vụ vào đơn {booking.Code}"
        }, cancellationToken);

        return new BookingServiceDetailDto
        {
            ServiceItemId = (int)service.Id,
            ServiceItemName = service.Name,
            Unit = service.Unit,
            Quantity = request.Quantity,
            UnitPrice = service.Price
        };
    }

    public async Task<PosCheckoutResponseDto> CheckoutAsync(PosCheckoutRequest request, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Branch)
            .Include(b => b.Slots)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy đơn đặt ID {request.BookingId}.");
        }

        if (booking.Status != BookingStatus.CheckedIn)
        {
            throw new InvalidOperationException($"Chỉ có thể thanh toán đơn đặt đang ở trạng thái Đang đá (CheckedIn). Trạng thái hiện tại: {booking.Status}.");
        }

        // Bổ sung các dịch vụ tại quầy nếu có
        if (request.AdditionalServices != null && request.AdditionalServices.Any())
        {
            foreach (var item in request.AdditionalServices)
            {
                await AddServiceAsync(new PosAddServiceRequest
                {
                    BookingId = request.BookingId,
                    ServiceItemId = item.ServiceItemId,
                    Quantity = item.Quantity,
                    PerformerName = request.PerformerName
                }, cancellationToken);
            }
        }

        // Áp dụng giảm giá
        if (request.DiscountAmount > 0)
        {
            booking.DiscountAmount = request.DiscountAmount;
            booking.TotalAmount = Math.Max(0, booking.SlotAmount + booking.ServiceAmount - booking.DiscountAmount);
        }

        decimal finalBalanceDue = Math.Max(0, booking.TotalAmount - booking.DepositAmount);

        if (request.PaymentMethod == PaymentMethod.Cash && request.CashGiven < finalBalanceDue)
        {
            throw new InvalidOperationException($"Số tiền khách đưa ({request.CashGiven:N0}đ) không đủ để thanh toán phần còn lại ({finalBalanceDue:N0}đ).");
        }

        // Tạo hóa đơn POS (Invoice)
        string invoiceCode = $"INV-{DateTime.UtcNow:yyyyMMdd}-{booking.Id:D4}";
        var invoice = new Invoice
        {
            BookingId = booking.Id,
            InvoiceNo = invoiceCode,
            TotalAmount = booking.TotalAmount,
            IssuedAt = DateTime.UtcNow,
            IssuedByName = request.PerformerName
        };

        // Chi tiết tiền sân trong hóa đơn
        invoice.Items.Add(new InvoiceItem
        {
            ItemType = InvoiceItemType.Slot,
            Description = $"Tiền thuê sân (Đơn {booking.Code})",
            Quantity = booking.Slots.Count,
            UnitPrice = booking.SlotAmount,
            Amount = booking.SlotAmount
        });

        // Chi tiết dịch vụ nếu có
        var bookingServices = await _context.BookingServices
            .Where(bs => bs.BookingId == booking.Id)
            .ToListAsync(cancellationToken);

        var serviceDetailsDto = new List<BookingServiceDetailDto>();
        foreach (var bs in bookingServices)
        {
            invoice.Items.Add(new InvoiceItem
            {
                ItemType = InvoiceItemType.Service,
                Description = bs.NameSnapshot,
                Quantity = bs.Quantity,
                UnitPrice = bs.UnitPriceSnapshot,
                Amount = bs.Amount
            });

            serviceDetailsDto.Add(new BookingServiceDetailDto
            {
                ServiceItemId = (int)bs.ServiceId,
                ServiceItemName = bs.NameSnapshot,
                Quantity = bs.Quantity,
                UnitPrice = bs.UnitPriceSnapshot
            });
        }

        _context.Invoices.Add(invoice);

        // Tạo giao dịch thanh toán Pha 2 (Payment)
        var payment = new Payment
        {
            BookingId = booking.Id,
            Type = PaymentType.Balance,
            Method = request.PaymentMethod,
            Amount = finalBalanceDue,
            Status = PaymentTransactionStatus.Success,
            ReceivedByName = request.PerformerName,
            PaidAt = DateTime.UtcNow,
            Note = request.Notes
        };
        _context.Payments.Add(payment);

        // Cập nhật trạng thái hoàn tất đơn
        booking.Status = BookingStatus.Completed;
        booking.PaymentStatus = BookingPaymentStatus.Paid;
        booking.PaidAmount = booking.DepositAmount + finalBalanceDue;
        booking.CompletedAt = DateTime.UtcNow;

        // Giải phóng slot locks
        var slotIds = booking.Slots.Select(s => s.Id).ToList();
        var locksToRemove = await _context.SlotLocks
            .Where(sl => sl.BookingSlotId.HasValue && slotIds.Contains(sl.BookingSlotId.Value))
            .ToListAsync(cancellationToken);
        if (locksToRemove.Any())
        {
            _context.SlotLocks.RemoveRange(locksToRemove);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Ghi Audit Log thanh toán POS
        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = request.PerformerName,
            BranchId = (int)booking.BranchId,
            Action = "POS_CHECKOUT",
            EntityName = "Invoice",
            EntityId = invoice.Id.ToString(),
            OldValue = $"Nợ: {finalBalanceDue:N0}đ",
            NewValue = $"Đã thanh toán: {finalBalanceDue:N0}đ qua {request.PaymentMethod}",
            Notes = $"Thanh toán hoàn tất hóa đơn {invoiceCode}"
        }, cancellationToken);

        decimal cashReturn = Math.Max(0, request.CashGiven - finalBalanceDue);

        return new PosCheckoutResponseDto
        {
            Success = true,
            Message = $"Thanh toán thành công hóa đơn {invoiceCode}",
            CashReturn = cashReturn,
            Invoice = new InvoiceDto
            {
                Id = (int)invoice.Id,
                InvoiceCode = invoiceCode,
                BookingId = (int)booking.Id,
                BookingCode = booking.Code,
                BranchId = (int)booking.BranchId,
                BranchName = booking.Branch?.Name ?? string.Empty,
                CustomerName = booking.Customer?.FullName ?? string.Empty,
                CustomerPhone = booking.Customer?.Phone ?? string.Empty,
                CourtFee = booking.SlotAmount,
                ServiceFee = booking.ServiceAmount,
                DiscountAmount = booking.DiscountAmount,
                DepositPaid = booking.DepositAmount,
                CashGiven = request.CashGiven,
                PaymentMethod = request.PaymentMethod,
                CashierName = request.PerformerName,
                CreatedAt = invoice.IssuedAt,
                ServiceDetails = serviceDetailsDto
            }
        };
    }

    public async Task<BookingDto?> GetPosBookingDetailsAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Branch)
            .Include(b => b.Slots)
                .ThenInclude(s => s.Court)
            .Include(b => b.Slots)
                .ThenInclude(s => s.Slot)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

        if (booking == null) return null;

        return new BookingDto
        {
            Id = (int)booking.Id,
            BookingCode = booking.Code,
            CustomerId = (int)booking.CustomerId,
            CustomerName = booking.Customer?.FullName ?? string.Empty,
            CustomerPhone = booking.Customer?.Phone ?? string.Empty,
            BranchId = (int)booking.BranchId,
            BranchName = booking.Branch?.Name ?? string.Empty,
            TotalAmount = booking.TotalAmount,
            DepositAmount = booking.DepositAmount,
            BookingStatus = booking.Status,
            PaymentStatus = (PaymentStatus)(int)booking.PaymentStatus,
            Notes = booking.Note,
            CreatedAt = booking.CreatedAt,
            Details = booking.Slots.Select(s => new BookingDetailDto
            {
                Id = (int)s.Id,
                BookingId = (int)s.BookingId,
                CourtId = (int)s.CourtId,
                CourtName = s.Court != null ? s.Court.Name : string.Empty,
                CourtType = (BTLtruc_quan.Shared.Enums.CourtType)(int)(s.Court != null ? s.Court.CourtTypeId : 1),
                BookingDate = s.SlotDate.ToDateTime(TimeOnly.MinValue),
                TimeSlotId = (int)s.SlotId,
                TimeSlotDisplayName = $"{s.StartAt:HH:mm} - {s.EndAt:HH:mm}",
                Price = s.PriceSnapshot
            }).ToList()
        };
    }
}
