using BTLtruc_quan.Shared.Enums;
using BTLtruc_quan.WebApi.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", t =>
        {
            t.HasCheckConstraint("CK_users_branch_role",
                "([role_id] = 2 AND [branch_id] IS NOT NULL) OR ([role_id] <> 2 AND [branch_id] IS NULL)");
            t.HasCheckConstraint("CK_users_no_show", "[no_show_count] >= 0");
            t.HasCheckConstraint("CK_users_status", ModelExtensions.EnumIn<UserStatus>("status"));
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(150).IsUnicode(false);
        builder.Property(x => x.Phone).HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.PasswordHash).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.AvatarUrl).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.LockReason).HasMaxLength(255);
        builder.Property(x => x.NoShowCount).HasDefaultValue(0);

        builder.HasIndex(x => x.Email).IsUnique().HasFilter("[email] IS NOT NULL");
        builder.HasIndex(x => x.Phone).IsUnique().HasFilter("[phone] IS NOT NULL");

        builder.HasOne(x => x.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AuthTokenConfiguration : IEntityTypeConfiguration<AuthToken>
{
    public void Configure(EntityTypeBuilder<AuthToken> builder)
    {
        builder.ToTable("auth_tokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TokenHash).HasMaxLength(255).IsUnicode(false).IsRequired();
        builder.Property(x => x.Type).AsStringEnum();

        builder.HasOne(x => x.User)
               .WithMany(u => u.AuthTokens)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BranchStaffNameConfiguration : IEntityTypeConfiguration<BranchStaffName>
{
    public void Configure(EntityTypeBuilder<BranchStaffName> builder)
    {
        builder.ToTable("branch_staff_names");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.StaffNames)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.BranchId, x.IsActive });
    }
}

public class CustomerViolationConfiguration : IEntityTypeConfiguration<CustomerViolation>
{
    public void Configure(EntityTypeBuilder<CustomerViolation> builder)
    {
        builder.ToTable("customer_violations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).AsStringEnum();
        builder.Property(x => x.ActionTaken).AsStringEnum();

        builder.HasOne(x => x.User)
               .WithMany(u => u.Violations)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Booking)
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResolvedByUser)
               .WithMany()
               .HasForeignKey(x => x.ResolvedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
