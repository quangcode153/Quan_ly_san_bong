using BTLtruc_quan.WebApi.Entities.Branches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches", t =>
        {
            t.HasCheckConstraint("CK_branches_open_close", "[open_time] < [close_time]");
            t.HasCheckConstraint("CK_branches_lat_long",
                "([latitude] IS NULL OR [latitude] BETWEEN -90 AND 90) AND ([longitude] IS NULL OR [longitude] BETWEEN -180 AND 180)");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(255).IsRequired();
        builder.Property(x => x.District).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Latitude).HasPrecision(10, 7);
        builder.Property(x => x.Longitude).HasPrecision(10, 7);
        builder.Property(x => x.Phone).HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.Description).HasColumnType("nvarchar(max)");
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.RejectReason).HasMaxLength(255);
        builder.Property(x => x.LockReason).HasMaxLength(255);
        builder.Property(x => x.AvgRating).HasPrecision(2, 1).HasDefaultValue(0.0m);

        builder.HasIndex(x => new { x.OwnerUserId, x.Name }).IsUnique();

        builder.HasOne(x => x.OwnerUser)
               .WithMany()
               .HasForeignKey(x => x.OwnerUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApprovedByUser)
               .WithMany()
               .HasForeignKey(x => x.ApprovedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BranchImageConfiguration : IEntityTypeConfiguration<BranchImage>
{
    public void Configure(EntityTypeBuilder<BranchImage> builder)
    {
        builder.ToTable("branch_images");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).HasMaxLength(255).IsUnicode(false).IsRequired();

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.Images)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("amenities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Icon).HasMaxLength(100).IsUnicode(false);
    }
}

public class BranchAmenityConfiguration : IEntityTypeConfiguration<BranchAmenity>
{
    public void Configure(EntityTypeBuilder<BranchAmenity> builder)
    {
        builder.ToTable("branch_amenities");
        builder.HasKey(x => new { x.BranchId, x.AmenityId });

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.BranchAmenities)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Amenity)
               .WithMany(a => a.BranchAmenities)
               .HasForeignKey(x => x.AmenityId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BranchPolicyConfiguration : IEntityTypeConfiguration<BranchPolicy>
{
    public void Configure(EntityTypeBuilder<BranchPolicy> builder)
    {
        builder.ToTable("branch_policies", t =>
        {
            t.HasCheckConstraint("CK_branch_policies_deposit", "[deposit_percent] BETWEEN 0 AND 100");
        });

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BranchId).IsUnique();

        builder.HasOne(x => x.Branch)
               .WithOne(b => b.Policy)
               .HasForeignKey<BranchPolicy>(x => x.BranchId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CancelPolicyTierConfiguration : IEntityTypeConfiguration<CancelPolicyTier>
{
    public void Configure(EntityTypeBuilder<CancelPolicyTier> builder)
    {
        builder.ToTable("cancel_policy_tiers", t =>
        {
            t.HasCheckConstraint("CK_cancel_policy_tiers_refund", "[refund_percent] BETWEEN 0 AND 100");
        });

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.BranchId, x.HoursBefore }).IsUnique();

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.CancelPolicyTiers)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
