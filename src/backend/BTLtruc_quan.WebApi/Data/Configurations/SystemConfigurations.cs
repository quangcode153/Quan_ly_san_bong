using BTLtruc_quan.WebApi.Entities.Extensions;
using BTLtruc_quan.WebApi.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Body).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Link).HasMaxLength(255).IsUnicode(false);

        builder.HasIndex(x => new { x.UserId, x.IsRead });

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BroadcastConfiguration : IEntityTypeConfiguration<Broadcast>
{
    public void Configure(EntityTypeBuilder<Broadcast> builder)
    {
        builder.ToTable("broadcasts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Target).AsStringEnum();

        builder.HasOne(x => x.SentByUser)
               .WithMany()
               .HasForeignKey(x => x.SentByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmailQueueConfiguration : IEntityTypeConfiguration<EmailQueue>
{
    public void Configure(EntityTypeBuilder<EmailQueue> builder)
    {
        builder.ToTable("email_queue");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ToEmail).HasMaxLength(150).IsUnicode(false).IsRequired();
        builder.Property(x => x.Template).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Body).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.Error).HasMaxLength(255);
    }
}

public class BookingReminderConfiguration : IEntityTypeConfiguration<BookingReminder>
{
    public void Configure(EntityTypeBuilder<BookingReminder> builder)
    {
        builder.ToTable("booking_reminders");
        builder.HasKey(x => new { x.BookingId, x.Type });
        builder.Property(x => x.Type).AsStringEnum();

        builder.HasOne(x => x.Booking)
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("system_settings");
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).HasMaxLength(100).IsUnicode(false);
        builder.Property(x => x.Value).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255);

        builder.HasOne(x => x.UpdatedByUser)
               .WithMany()
               .HasForeignKey(x => x.UpdatedByUserId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ActorRole).HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.PerformedByName).HasMaxLength(100);
        builder.Property(x => x.Action).HasMaxLength(80).IsUnicode(false).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(x => x.OldValue).HasColumnType("nvarchar(max)");
        builder.Property(x => x.NewValue).HasColumnType("nvarchar(max)");
        builder.Property(x => x.Ip).HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.UserAgent).HasMaxLength(255).IsUnicode(false);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => new { x.BranchId, x.CreatedAt });

        builder.HasOne(x => x.ActorUser)
               .WithMany()
               .HasForeignKey(x => x.ActorUserId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}

public class JobRunConfiguration : IEntityTypeConfiguration<JobRun>
{
    public void Configure(EntityTypeBuilder<JobRun> builder)
    {
        builder.ToTable("job_runs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.JobName).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.Error).HasMaxLength(255);
    }
}

public class WaitlistConfiguration : IEntityTypeConfiguration<Waitlist>
{
    public void Configure(EntityTypeBuilder<Waitlist> builder)
    {
        builder.ToTable("waitlist");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).AsStringEnum();

        builder.HasIndex(x => new { x.CustomerId, x.CourtId, x.SlotDate, x.SlotId })
               .IsUnique()
               .HasFilter("[status] = 'Waiting'");

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
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

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("favorites");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.CustomerId, x.BranchId, x.CourtId }).IsUnique();

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
