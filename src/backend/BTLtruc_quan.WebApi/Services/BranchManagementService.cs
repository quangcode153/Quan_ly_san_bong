using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Data;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.Services;
using Microsoft.EntityFrameworkCore;

namespace BTLtruc_quan.WebApi.Services;

/// <summary>
/// Hiện thực hóa dịch vụ Quản trị cơ sở Chi nhánh (Sân, Bảng giá ca, Dịch vụ kho)
/// Căn cứ: doc/Đặc tả chức năng - Hệ thống quản lý chuỗi cho thuê sân thể thao.md (QT05, QT11, QT18)
/// </summary>
public class BranchManagementService : IBranchManagementService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public BranchManagementService(AppDbContext context, IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    // =========================================================================
    // 1. QUẢN LÝ SÂN THI ĐẤU
    // =========================================================================

    public async Task<List<CourtDto>> GetCourtsByBranchAsync(int branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Courts
            .Where(c => c.BranchId == branchId && c.DeletedAt == null)
            .OrderBy(c => c.Name)
            .Select(c => new CourtDto
            {
                Id = (int)c.Id,
                BranchId = (int)c.BranchId,
                Code = $"CRT-{c.Id:D3}",
                Name = c.Name,
                CourtType = (BTLtruc_quan.Shared.Enums.CourtType)(int)c.CourtTypeId,
                SurfaceType = "Tiêu chuẩn",
                Status = c.Status,
                Description = c.Description,
                DefaultPricePerHour = 300000m
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CourtDto> CreateCourtAsync(CreateCourtRequest request, string performerName, CancellationToken cancellationToken = default)
    {
        var court = new Court
        {
            BranchId = request.BranchId,
            Name = request.Name,
            CourtTypeId = (long)(int)request.CourtType,
            Description = request.Description,
            Status = CourtStatus.Active,
            Capacity = 10
        };

        _context.Courts.Add(court);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = request.BranchId,
            Action = "CREATE_COURT",
            EntityName = "Court",
            EntityId = court.Id.ToString(),
            OldValue = null,
            NewValue = $"Tạo sân mới {court.Name}",
            Notes = $"Thêm sân thể thao mới tại chi nhánh {request.BranchId}"
        }, cancellationToken);

        return new CourtDto
        {
            Id = (int)court.Id,
            BranchId = (int)court.BranchId,
            Code = string.IsNullOrWhiteSpace(request.Code) ? $"CRT-{court.Id:D3}" : request.Code,
            Name = court.Name,
            CourtType = request.CourtType,
            SurfaceType = request.SurfaceType ?? "Tiêu chuẩn",
            Status = CourtStatus.Active,
            Description = court.Description,
            DefaultPricePerHour = request.DefaultPricePerHour
        };
    }

    public async Task<CourtDto> UpdateCourtAsync(int courtId, UpdateCourtRequest request, string performerName, CancellationToken cancellationToken = default)
    {
        var court = await _context.Courts.FindAsync(new object[] { (long)courtId }, cancellationToken);
        if (court == null || court.DeletedAt != null)
        {
            throw new KeyNotFoundException($"Không tìm thấy sân thể thao ID {courtId}.");
        }

        string oldInfo = $"{court.Name}";
        court.Name = request.Name;
        court.CourtTypeId = (long)(int)request.CourtType;
        court.Status = request.Status;
        court.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = (int)court.BranchId,
            Action = "UPDATE_COURT",
            EntityName = "Court",
            EntityId = court.Id.ToString(),
            OldValue = oldInfo,
            NewValue = $"{court.Name}",
            Notes = $"Cập nhật thông tin sân #{courtId}"
        }, cancellationToken);

        return new CourtDto
        {
            Id = (int)court.Id,
            BranchId = (int)court.BranchId,
            Code = $"CRT-{court.Id:D3}",
            Name = court.Name,
            CourtType = request.CourtType,
            SurfaceType = request.SurfaceType ?? "Tiêu chuẩn",
            Status = request.Status,
            Description = court.Description,
            DefaultPricePerHour = request.DefaultPricePerHour
        };
    }

    public async Task<bool> SetCourtMaintenanceStatusAsync(int courtId, bool isMaintenance, string performerName, CancellationToken cancellationToken = default)
    {
        var court = await _context.Courts.FindAsync(new object[] { (long)courtId }, cancellationToken);
        if (court == null || court.DeletedAt != null) return false;

        var targetStatus = isMaintenance 
            ? CourtStatus.Maintenance 
            : CourtStatus.Active;

        string oldStatus = court.Status.ToString();
        court.Status = targetStatus;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = (int)court.BranchId,
            Action = "MAINTENANCE_CHANGE",
            EntityName = "Court",
            EntityId = court.Id.ToString(),
            OldValue = oldStatus,
            NewValue = targetStatus.ToString(),
            Notes = isMaintenance ? "Khóa sân để bảo trì kỹ thuật" : "Mở lại sân hoạt động bình thường"
        }, cancellationToken);

        return true;
    }

    public async Task<bool> SoftDeleteCourtAsync(int courtId, string performerName, CancellationToken cancellationToken = default)
    {
        var court = await _context.Courts.FindAsync(new object[] { (long)courtId }, cancellationToken);
        if (court == null || court.DeletedAt != null) return false;

        // Xóa mềm theo QT11
        court.DeletedAt = DateTime.UtcNow;
        court.Status = CourtStatus.Inactive;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = (int)court.BranchId,
            Action = "DELETE_COURT",
            EntityName = "Court",
            EntityId = court.Id.ToString(),
            OldValue = "Active",
            NewValue = "SoftDeleted",
            Notes = $"Xóa mềm sân {court.Name}"
        }, cancellationToken);

        return true;
    }

    // =========================================================================
    // 2. CẤU HÌNH BẢNG GIÁ CA SÂN (QT05)
    // =========================================================================

    public async Task<List<CourtPriceDto>> GetPriceRulesAsync(int branchId, CancellationToken cancellationToken = default)
    {
        return await _context.PriceRules
            .Where(p => p.BranchId == branchId)
            .OrderBy(p => p.CourtTypeId)
            .Select(p => new CourtPriceDto
            {
                Id = (int)p.Id,
                BranchId = (int)p.BranchId,
                CourtType = (BTLtruc_quan.Shared.Enums.CourtType)(int)(p.CourtTypeId ?? 1),
                TimeSlotId = (int)p.SlotType,
                PricePerHour = p.Price,
                IsPeakHour = p.SlotType == SlotType.Peak
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CourtPriceDto> SetCourtPriceAsync(SetCourtPriceRequest request, string performerName, CancellationToken cancellationToken = default)
    {
        bool isPeak = request.TimeSlotId >= 12; // Sau 17:00 là giờ cao điểm (QT05)
        var targetSlotType = isPeak ? SlotType.Peak : SlotType.OffPeak;

        var rule = await _context.PriceRules
            .FirstOrDefaultAsync(p => p.BranchId == request.BranchId && 
                                     p.CourtTypeId == (long)(int)request.CourtType &&
                                     p.SlotType == targetSlotType, cancellationToken);

        decimal oldPrice = 0;

        if (rule == null)
        {
            rule = new PriceRule
            {
                BranchId = request.BranchId,
                CourtTypeId = (long)(int)request.CourtType,
                SlotType = targetSlotType,
                DayType = request.SpecificDayOfWeek.HasValue && (request.SpecificDayOfWeek == DayOfWeek.Saturday || request.SpecificDayOfWeek == DayOfWeek.Sunday) 
                    ? DayType.Weekend 
                    : DayType.Weekday,
                Price = request.PricePerHour,
                ValidFrom = DateOnly.FromDateTime(DateTime.Today),
                ValidTo = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
            };
            _context.PriceRules.Add(rule);
        }
        else
        {
            oldPrice = rule.Price;
            rule.Price = request.PricePerHour;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = request.BranchId,
            Action = "UPDATE_PRICE",
            EntityName = "PriceRule",
            EntityId = rule.Id.ToString(),
            OldValue = $"{oldPrice:N0}đ",
            NewValue = $"{request.PricePerHour:N0}đ",
            Notes = $"Đổi bảng giá loại sân {request.CourtType} - {(isPeak ? "Giờ cao điểm" : "Giờ thường")}"
        }, cancellationToken);

        return new CourtPriceDto
        {
            Id = (int)rule.Id,
            BranchId = (int)rule.BranchId,
            CourtType = request.CourtType,
            TimeSlotId = request.TimeSlotId,
            PricePerHour = rule.Price,
            IsPeakHour = isPeak
        };
    }

    // =========================================================================
    // 3. QUẢN LÝ DỊCH VỤ & TỒN KHO
    // =========================================================================

    public async Task<List<ServiceItemDto>> GetServiceItemsAsync(int branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Where(s => s.BranchId == branchId && s.DeletedAt == null)
            .OrderBy(s => s.Name)
            .Select(s => new ServiceItemDto
            {
                Id = (int)s.Id,
                BranchId = (int)s.BranchId,
                Code = $"SVC-{s.Id:D3}",
                Name = s.Name,
                Price = s.Price,
                Unit = s.Unit,
                StockQuantity = s.StockQty ?? 0,
                IsActive = s.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceItemDto> CreateServiceItemAsync(CreateServiceItemRequest request, string performerName, CancellationToken cancellationToken = default)
    {
        var service = new Service
        {
            BranchId = request.BranchId,
            Name = request.Name,
            Unit = request.Unit,
            Price = request.Price,
            StockQty = request.StockQuantity,
            IsActive = true
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = request.BranchId,
            Action = "CREATE_SERVICE",
            EntityName = "Service",
            EntityId = service.Id.ToString(),
            OldValue = null,
            NewValue = $"Thêm mặt hàng {service.Name} (Kho: {service.StockQty})",
            Notes = $"Thêm dịch vụ mới tại chi nhánh #{request.BranchId}"
        }, cancellationToken);

        return new ServiceItemDto
        {
            Id = (int)service.Id,
            BranchId = (int)service.BranchId,
            Code = $"SVC-{service.Id:D3}",
            Name = service.Name,
            Price = service.Price,
            Unit = service.Unit,
            StockQuantity = service.StockQty ?? 0,
            IsActive = true
        };
    }

    public async Task<ServiceItemDto> UpdateServiceItemAsync(int serviceItemId, UpdateServiceItemRequest request, string performerName, CancellationToken cancellationToken = default)
    {
        var service = await _context.Services.FindAsync(new object[] { (long)serviceItemId }, cancellationToken);
        if (service == null || service.DeletedAt != null)
        {
            throw new KeyNotFoundException($"Không tìm thấy dịch vụ ID {serviceItemId}.");
        }

        string oldVal = $"{service.Name} - {service.Price:N0}đ (Kho: {service.StockQty})";
        service.Name = request.Name;
        service.Price = request.Price;
        service.Unit = request.Unit;
        service.StockQty = request.StockQuantity;
        service.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = (int)service.BranchId,
            Action = "UPDATE_SERVICE",
            EntityName = "Service",
            EntityId = service.Id.ToString(),
            OldValue = oldVal,
            NewValue = $"{service.Name} - {service.Price:N0}đ (Kho: {service.StockQty})",
            Notes = $"Cập nhật dịch vụ #{serviceItemId}"
        }, cancellationToken);

        return new ServiceItemDto
        {
            Id = (int)service.Id,
            BranchId = (int)service.BranchId,
            Code = $"SVC-{service.Id:D3}",
            Name = service.Name,
            Price = service.Price,
            Unit = service.Unit,
            StockQuantity = service.StockQty ?? 0,
            IsActive = service.IsActive
        };
    }

    public async Task<bool> UpdateStockQuantityAsync(int serviceItemId, int newStock, string performerName, CancellationToken cancellationToken = default)
    {
        var service = await _context.Services.FindAsync(new object[] { (long)serviceItemId }, cancellationToken);
        if (service == null || service.DeletedAt != null) return false;

        int oldStock = service.StockQty ?? 0;
        service.StockQty = newStock;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new CreateAuditLogRequest
        {
            PerformerName = performerName,
            BranchId = (int)service.BranchId,
            Action = "STOCK_ADJUSTMENT",
            EntityName = "Service",
            EntityId = service.Id.ToString(),
            OldValue = $"Tồn kho cũ: {oldStock}",
            NewValue = $"Tồn kho mới: {newStock}",
            Notes = $"Điều chỉnh tồn kho mặt hàng {service.Name}"
        }, cancellationToken);

        return true;
    }
}
