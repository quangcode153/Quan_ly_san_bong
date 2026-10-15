using System.Linq.Expressions;
using BTLtruc_quan.WebApi.Entities;
using BTLtruc_quan.WebApi.Entities.Bookings;
using BTLtruc_quan.WebApi.Entities.Branches;
using BTLtruc_quan.WebApi.Entities.Courts;
using BTLtruc_quan.WebApi.Entities.Extensions;
using BTLtruc_quan.WebApi.Entities.Interactions;
using BTLtruc_quan.WebApi.Entities.Payments;
using BTLtruc_quan.WebApi.Entities.Services;
using BTLtruc_quan.WebApi.Entities.System;
using BTLtruc_quan.WebApi.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace BTLtruc_quan.WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Nhóm A: Người dùng & Xác thực
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AuthToken> AuthTokens => Set<AuthToken>();
    public DbSet<BranchStaffName> BranchStaffNames => Set<BranchStaffName>();
    public DbSet<CustomerViolation> CustomerViolations => Set<CustomerViolation>();

    // Nhóm B: Chi nhánh & Tiện ích
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<BranchImage> BranchImages => Set<BranchImage>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<BranchAmenity> BranchAmenities => Set<BranchAmenity>();
    public DbSet<BranchPolicy> BranchPolicies => Set<BranchPolicy>();
    public DbSet<CancelPolicyTier> CancelPolicyTiers => Set<CancelPolicyTier>();

    // Nhóm C: Sân, Ca & Bảng giá
    public DbSet<CourtType> CourtTypes => Set<CourtType>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<PriceRule> PriceRules => Set<PriceRule>();
    public DbSet<SlotBlock> SlotBlocks => Set<SlotBlock>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    // Nhóm D: Đơn đặt sân & Khóa ca chống trùng (Cốt lõi)
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSlot> BookingSlots => Set<BookingSlot>();
    public DbSet<SlotLock> SlotLocks => Set<SlotLock>();
    public DbSet<BookingHistory> BookingHistories => Set<BookingHistory>();
    public DbSet<RecurringBooking> RecurringBookings => Set<RecurringBooking>();
    public DbSet<RecurringSkip> RecurringSkips => Set<RecurringSkip>();

    // Nhóm E: Thanh toán & Hóa đơn
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<DailyClosing> DailyClosings => Set<DailyClosing>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();

    // Nhóm F: Dịch vụ & Thiết bị
    public DbSet<Service> Services => Set<Service>();
    public DbSet<BookingService> BookingServices => Set<BookingService>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Incident> Incidents => Set<Incident>();

    // Nhóm G: Tương tác
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewReply> ReviewReplies => Set<ReviewReply>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<ComplaintMessage> ComplaintMessages => Set<ComplaintMessage>();
    public DbSet<ChatThread> ChatThreads => Set<ChatThread>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    // Nhóm H: Hệ thống & Nhật ký
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Broadcast> Broadcasts => Set<Broadcast>();
    public DbSet<EmailQueue> EmailQueue => Set<EmailQueue>();
    public DbSet<BookingReminder> BookingReminders => Set<BookingReminder>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<JobRun> JobRuns => Set<JobRun>();

    // Nhóm I: Mở rộng
    public DbSet<Waitlist> Waitlists => Set<Waitlist>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().HaveMaxLength(255);
        configurationBuilder.Properties<decimal>().HavePrecision(12, 0);
        configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2(0)");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_100_CI_AS");

        // 1. Mặc định RESTRICT cho mọi khóa ngoại trước khi ApplyConfigurations
        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // 2. Nạp toàn bộ configurations từ assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // 3. Cấu hình Global Query Filter cho các entity xóa mềm (ISoftDelete)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDelete.DeletedAt));
                var isNull = Expression.Equal(property, Expression.Constant(null, typeof(DateTime?)));
                var lambda = Expression.Lambda(isNull, parameter);
                entityType.SetQueryFilter(lambda);
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        // Xử lý xóa mềm: Chuyển Delete thành Update DeletedAt
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>().Where(x => x.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.DeletedAt = now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
