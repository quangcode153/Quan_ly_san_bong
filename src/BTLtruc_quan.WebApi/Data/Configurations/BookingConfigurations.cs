using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings", t =>
        {
            t.HasCheckConstraint("CK_bookings_time", "[end_at] > [start_at]");
            t.HasCheckConstraint("CK_bookings_money",
                "[slot_amount] >= 0 AND [service_amount] >= 0 AND [discount_amount] >= 0 AND [deposit_amount] >= 0 AND [paid_amount] >= 0 AND [refund_amount] >= 0");
            t.HasCheckConstraint("CK_bookings_total", "[total_amount] = [slot_amount] + [service_amount] - [discount_amount]");
            t.HasCheckConstraint("CK_bookings_deposit", "[deposit_amount] <= [total_amount] AND [refund_amount] <= [paid_amount]");
            t.HasCheckConstraint("CK_bookings_percent", "[deposit_percent_snapshot] BETWEEN 0 AND 100");
            t.HasCheckConstraint("CK_bookings_hold", "[status] <> 'Holding' OR [hold_expires_at] IS NOT NULL");
            t.HasCheckConstraint("CK_bookings_cancel",
                "[status] NOT IN ('Cancelled','Expired','NoShow') OR ([cancel_type] IS NOT NULL AND [cancelled_at] IS NOT NULL)");
            t.HasCheckConstraint("CK_bookings_cancel_reason",
                "[cancel_type] NOT IN ('Manager','Admin') OR LEN(LTRIM(RTRIM([cancel_reason]))) > 0");
            t.HasCheckConstraint("CK_bookings_checkin", "[status] NOT IN ('CheckedIn','Completed') OR [checked_in_at] IS NOT NULL");
            t.HasCheckConstraint("CK_bookings_status", ModelExtensions.EnumIn<BookingStatus>("status"));
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.Source).AsStringEnum();
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.PaymentStatus).AsStringEnum();
        builder.Property(x => x.QrSecret).HasMaxLength(64).IsUnicode(false).IsRequired();
        builder.Property(x => x.CancelPolicySnapshot).HasColumnType("nvarchar(max)");
        builder.Property(x => x.CheckinMethod).AsStringEnum();
        builder.Property(x => x.CheckedInByName).HasMaxLength(100);
        builder.Property(x => x.CancelType).AsStringEnum();
        builder.Property(x => x.CancelReason).HasMaxLength(255);
        builder.Property(x => x.CancelledByName).HasMaxLength(100);
        builder.Property(x => x.CreatedByName).HasMaxLength(100);
        builder.Property(x => x.Note).HasMaxLength(255);

        // Optimistic Concurrency Control
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Indexes
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => new { x.Status, x.HoldExpiresAt });
        builder.HasIndex(x => new { x.Status, x.StartAt });
        builder.HasIndex(x => new { x.BranchId, x.PlayDate });

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RecurringBooking)
               .WithMany(rb => rb.Bookings)
               .HasForeignKey(x => x.RecurringId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Voucher)
               .WithMany()
               .HasForeignKey(x => x.VoucherId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CancelledByUser)
               .WithMany()
               .HasForeignKey(x => x.CancelledByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
               .WithMany()
               .HasForeignKey(x => x.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BookingSlotConfiguration : IEntityTypeConfiguration<BookingSlot>
{
    public void Configure(EntityTypeBuilder<BookingSlot> builder)
    {
        builder.ToTable("booking_slots", t =>
        {
            t.HasCheckConstraint("CK_booking_slots_time", "[end_at] > [start_at]");
            t.HasCheckConstraint("CK_booking_slots_price", "[price_snapshot] >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.SlotTypeSnapshot).AsStringEnum();
        builder.Property(x => x.DayTypeSnapshot).AsStringEnum();
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.PriceSnapshot).HasPrecision(12, 0);

        builder.HasIndex(x => new { x.CourtId, x.SlotDate });
        builder.HasIndex(x => new { x.BookingId, x.CourtId, x.SlotDate, x.SlotId }).IsUnique();

        builder.HasOne(x => x.Booking)
               .WithMany(b => b.Slots)
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Slot)
               .WithMany()
               .HasForeignKey(x => x.SlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SlotLockConfiguration : IEntityTypeConfiguration<SlotLock>
{
    public void Configure(EntityTypeBuilder<SlotLock> builder)
    {
        builder.ToTable("slot_locks", t =>
        {
            t.HasCheckConstraint("CK_slot_locks_source",
                "([source_type] = 'Booking' AND [booking_slot_id] IS NOT NULL AND [block_id] IS NULL) OR " +
                "([source_type] = 'Block' AND [block_id] IS NOT NULL AND [booking_slot_id] IS NULL)");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceType).AsStringEnum();

        // Optimistic Concurrency Control
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Ràng buộc cốt lõi chống trùng lịch (QT02)
        builder.HasIndex(x => new { x.CourtId, x.SlotDate, x.SlotId }).IsUnique();
        builder.HasIndex(x => x.BookingSlotId).IsUnique().HasFilter("[booking_slot_id] IS NOT NULL");

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Slot)
               .WithMany()
               .HasForeignKey(x => x.SlotId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BookingSlot)
               .WithOne(bs => bs.SlotLock)
               .HasForeignKey<SlotLock>(x => x.BookingSlotId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Block)
               .WithMany(sb => sb.Locks)
               .HasForeignKey(x => x.BlockId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BookingHistoryConfiguration : IEntityTypeConfiguration<BookingHistory>
{
    public void Configure(EntityTypeBuilder<BookingHistory> builder)
    {
        builder.ToTable("booking_history");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).AsStringEnum();
        builder.Property(x => x.OldValue).HasColumnType("nvarchar(max)");
        builder.Property(x => x.NewValue).HasColumnType("nvarchar(max)");
        builder.Property(x => x.PriceDiff).HasPrecision(12, 0);
        builder.Property(x => x.PerformedByName).HasMaxLength(100);

        builder.HasOne(x => x.Booking)
               .WithMany(b => b.History)
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PerformedByUser)
               .WithMany()
               .HasForeignKey(x => x.PerformedByUserId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}

public class RecurringBookingConfiguration : IEntityTypeConfiguration<RecurringBooking>
{
    public void Configure(EntityTypeBuilder<RecurringBooking> builder)
    {
        builder.ToTable("recurring_bookings", t =>
        {
            t.HasCheckConstraint("CK_recurring_bookings_weekday", "[weekday] BETWEEN 1 AND 7");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).AsStringEnum();

        builder.HasIndex(x => new { x.CustomerId, x.CourtId, x.Weekday, x.SlotId })
               .IsUnique()
               .HasFilter("[status] = 'Active'");

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Slot)
               .WithMany()
               .HasForeignKey(x => x.SlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RecurringSkipConfiguration : IEntityTypeConfiguration<RecurringSkip>
{
    public void Configure(EntityTypeBuilder<RecurringSkip> builder)
    {
        builder.ToTable("recurring_skips");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).AsStringEnum();

        builder.HasIndex(x => new { x.RecurringId, x.SkipDate }).IsUnique();

        builder.HasOne(x => x.RecurringBooking)
               .WithMany(rb => rb.Skips)
               .HasForeignKey(x => x.RecurringId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
