using BTLtruc_quan.WebApi.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services", t =>
        {
            t.HasCheckConstraint("CK_services_price", "[price] >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Unit).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Price).HasPrecision(12, 0);

        builder.HasIndex(x => new { x.BranchId, x.Name }).IsUnique().HasFilter("[deleted_at] IS NULL");

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.Services)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BookingServiceConfiguration : IEntityTypeConfiguration<BookingService>
{
    public void Configure(EntityTypeBuilder<BookingService> builder)
    {
        builder.ToTable("booking_services", t =>
        {
            t.HasCheckConstraint("CK_booking_services_qty", "[quantity] > 0 AND [unit_price_snapshot] >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.NameSnapshot).HasMaxLength(100).IsRequired();
        builder.Property(x => x.UnitPriceSnapshot).HasPrecision(12, 0);
        builder.Property(x => x.Amount).HasPrecision(12, 0);
        builder.Property(x => x.AddedWhen).AsStringEnum();
        builder.Property(x => x.AddedByName).HasMaxLength(100);

        builder.HasOne(x => x.Booking)
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Service)
               .WithMany()
               .HasForeignKey(x => x.ServiceId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("equipment");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.Note).HasMaxLength(255);

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("incidents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Severity).AsStringEnum();
        builder.Property(x => x.ImageUrl).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.ReportedByName).HasMaxLength(100);
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.ResolvedNote).HasMaxLength(255);

        builder.HasOne(x => x.Court)
               .WithMany()
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Equipment)
               .WithMany()
               .HasForeignKey(x => x.EquipmentId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
