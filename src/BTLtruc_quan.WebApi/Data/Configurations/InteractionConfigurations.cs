using BTLtruc_quan.WebApi.Entities.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews", t =>
        {
            t.HasCheckConstraint("CK_reviews_rating", "[rating] BETWEEN 1 AND 5");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Comment).HasColumnType("nvarchar(max)");

        builder.HasIndex(x => x.BookingId).IsUnique();

        builder.HasOne(x => x.Booking)
               .WithOne(b => b.Review)
               .HasForeignKey<Review>(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

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
    }
}

public class ReviewReplyConfiguration : IEntityTypeConfiguration<ReviewReply>
{
    public void Configure(EntityTypeBuilder<ReviewReply> builder)
    {
        builder.ToTable("review_replies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();

        builder.HasIndex(x => x.ReviewId).IsUnique();

        builder.HasOne(x => x.Review)
               .WithOne(r => r.Reply)
               .HasForeignKey<ReviewReply>(x => x.ReviewId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.ToTable("complaints");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.Resolution).HasColumnType("nvarchar(max)");

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Booking)
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.ResolvedByUser)
               .WithMany()
               .HasForeignKey(x => x.ResolvedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ComplaintMessageConfiguration : IEntityTypeConfiguration<ComplaintMessage>
{
    public void Configure(EntityTypeBuilder<ComplaintMessage> builder)
    {
        builder.ToTable("complaint_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SenderRole).AsStringEnum();
        builder.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.AttachmentUrls).HasColumnType("nvarchar(max)");

        builder.HasOne(x => x.Complaint)
               .WithMany(c => c.Messages)
               .HasForeignKey(x => x.ComplaintId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Sender)
               .WithMany()
               .HasForeignKey(x => x.SenderId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ChatThreadConfiguration : IEntityTypeConfiguration<ChatThread>
{
    public void Configure(EntityTypeBuilder<ChatThread> builder)
    {
        builder.ToTable("chat_threads");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).AsStringEnum();
        builder.Property(x => x.Status).HasMaxLength(30).IsUnicode(false);

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chat_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SenderType).AsStringEnum();
        builder.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.Meta).HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new { x.ThreadId, x.CreatedAt });

        builder.HasOne(x => x.Thread)
               .WithMany(t => t.Messages)
               .HasForeignKey(x => x.ThreadId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Sender)
               .WithMany()
               .HasForeignKey(x => x.SenderId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
