using BTLtruc_quan.Shared.DTOs;
using BTLtruc_quan.WebApi.Data;
using BTLtruc_quan.WebApi.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace BTLtruc_quan.WebApi.Services;

/// <summary>
/// Hiện thực hóa dịch vụ Nhật ký kiểm toán hoạt động (Audit Log - QT18 & Mục 1.4)
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(CreateAuditLogRequest request, CancellationToken cancellationToken = default)
    {
        long entityIdLong = 0;
        long.TryParse(request.EntityId, out entityIdLong);

        var auditLog = new AuditLog
        {
            ActorUserId = request.ActorUserId.HasValue ? request.ActorUserId.Value : null,
            ActorRole = "STAFF",
            PerformedByName = request.PerformerName,
            BranchId = request.BranchId > 0 ? request.BranchId : null,
            Action = request.Action,
            EntityType = request.EntityName,
            EntityId = entityIdLong,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AuditLogDto>> GetLogsAsync(AuditLogFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Include(a => a.Branch)
            .AsNoTracking();

        if (filter.BranchId.HasValue && filter.BranchId > 0)
        {
            query = query.Where(a => a.BranchId == filter.BranchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Action))
        {
            query = query.Where(a => a.Action == filter.Action);
        }

        if (!string.IsNullOrWhiteSpace(filter.EntityName))
        {
            query = query.Where(a => a.EntityType == filter.EntityName);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= filter.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
        {
            string kw = filter.SearchKeyword.ToLower();
            query = query.Where(a => (a.PerformedByName != null && a.PerformedByName.ToLower().Contains(kw)) ||
                                     a.Action.ToLower().Contains(kw) ||
                                     a.EntityType.ToLower().Contains(kw));
        }

        var pagedLogs = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((filter.PageIndex - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                ActorUserId = a.ActorUserId.HasValue ? (int)a.ActorUserId.Value : null,
                PerformerName = a.PerformedByName ?? string.Empty,
                BranchId = a.BranchId.HasValue ? (int)a.BranchId.Value : 0,
                BranchName = a.Branch != null ? a.Branch.Name : string.Empty,
                Action = a.Action,
                EntityName = a.EntityType,
                EntityId = a.EntityId.ToString(),
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return pagedLogs;
    }
}

