using BTLtruc_quan.WebApi.Entities.Courts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class CourtTypeConfiguration : IEntityTypeConfiguration<CourtType>
{
    public void Configure(EntityTypeBuilder<CourtType> builder)
    {
        builder.ToTable("court_types");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Icon).HasMaxLength(100).IsUnicode(false);
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

public class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.ToTable("courts", t =>
        {
            t.HasCheckConstraint("CK_courts_capacity", "[capacity] > 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ImageUrl).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.LiveStatus).AsStringEnum();

        builder.HasIndex(x => new { x.BranchId, x.Name }).IsUnique().HasFilter("[deleted_at] IS NULL");

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.Courts)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CourtType)
               .WithMany(ct => ct.Courts)
               .HasForeignKey(x => x.CourtTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("time_slots");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SlotType).AsStringEnum();

        builder.HasIndex(x => new { x.BranchId, x.SlotIndex }).IsUnique();
        builder.HasIndex(x => new { x.BranchId, x.StartTime }).IsUnique();

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.TimeSlots)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PriceRuleConfiguration : IEntityTypeConfiguration<PriceRule>
{
    public void Configure(EntityTypeBuilder<PriceRule> builder)
    {
        builder.ToTable("price_rules", t =>
        {
            t.HasCheckConstraint("CK_price_rules_price", "[price] >= 0");
            t.HasCheckConstraint("CK_price_rules_dates", "[valid_to] IS NULL OR [valid_to] >= [valid_from]");
            t.HasCheckConstraint("CK_price_rules_target", "[court_id] IS NOT NULL OR [court_type_id] IS NOT NULL");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.DayType).AsStringEnum();
        builder.Property(x => x.SlotType).AsStringEnum();
        builder.Property(x => x.Price).HasPrecision(12, 0);

        builder.HasIndex(x => new { x.BranchId, x.CourtTypeId, x.CourtId, x.DayType, x.SlotType, x.ValidFrom })
               .IsUnique()
               .HasFilter(null);

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.PriceRules)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CourtType)
               .WithMany()
               .HasForeignKey(x => x.CourtTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Court)
               .WithMany(c => c.PriceRules)
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SlotBlockConfiguration : IEntityTypeConfiguration<SlotBlock>
{
    public void Configure(EntityTypeBuilder<SlotBlock> builder)
    {
        builder.ToTable("slot_blocks", t =>
        {
            t.HasCheckConstraint("CK_slot_blocks_dates", "[end_date] >= [start_date]");
            t.HasCheckConstraint("CK_slot_blocks_slots",
                "([start_slot_id] IS NULL AND [end_slot_id] IS NULL) OR ([start_slot_id] IS NOT NULL AND [end_slot_id] IS NOT NULL)");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.BlockType).AsStringEnum();
        builder.Property(x => x.Reason).HasMaxLength(255);
        builder.Property(x => x.PerformedByName).HasMaxLength(100);

        builder.HasOne(x => x.Court)
               .WithMany(c => c.SlotBlocks)
               .HasForeignKey(x => x.CourtId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StartSlot)
               .WithMany()
               .HasForeignKey(x => x.StartSlotId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EndSlot)
               .WithMany()
               .HasForeignKey(x => x.EndSlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.ToTable("holidays", t =>
        {
            t.HasCheckConstraint("CK_holidays_surcharge", "[surcharge_percent] >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.BranchId, x.HolidayDate }).IsUnique();

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
