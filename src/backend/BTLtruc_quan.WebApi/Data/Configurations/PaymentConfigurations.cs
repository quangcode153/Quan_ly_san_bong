using BTLtruc_quan.WebApi.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BTLtruc_quan.WebApi.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", t =>
        {
            t.HasCheckConstraint("CK_payments_amount", "[amount] >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).AsStringEnum();
        builder.Property(x => x.Method).AsStringEnum();
        builder.Property(x => x.Gateway).AsStringEnum();
        builder.Property(x => x.Status).AsStringEnum();
        builder.Property(x => x.Amount).HasPrecision(12, 0);
        builder.Property(x => x.GatewayTxnId).HasMaxLength(100).IsUnicode(false);
        builder.Property(x => x.GatewayPayload).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ReceivedByName).HasMaxLength(100);
        builder.Property(x => x.Note).HasMaxLength(255);

        builder.HasIndex(x => x.GatewayTxnId).IsUnique().HasFilter("[gateway_txn_id] IS NOT NULL");

        builder.HasOne(x => x.Booking)
               .WithMany(b => b.Payments)
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedPayment)
               .WithMany()
               .HasForeignKey(x => x.RelatedPaymentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices", t =>
        {
            t.HasCheckConstraint("CK_invoices_total", "[total_amount] >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.InvoiceNo).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(12, 0);
        builder.Property(x => x.IssuedByName).HasMaxLength(100);
        builder.Property(x => x.PdfPath).HasMaxLength(255).IsUnicode(false);

        builder.HasIndex(x => x.InvoiceNo).IsUnique();
        builder.HasIndex(x => x.BookingId).IsUnique();

        builder.HasOne(x => x.Booking)
               .WithOne(b => b.Invoice)
               .HasForeignKey<Invoice>(x => x.BookingId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("invoice_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ItemType).AsStringEnum();
        builder.Property(x => x.Description).HasMaxLength(255).IsRequired();
        builder.Property(x => x.UnitPrice).HasPrecision(12, 0);
        builder.Property(x => x.Amount).HasPrecision(12, 0);

        builder.HasOne(x => x.Invoice)
               .WithMany(i => i.Items)
               .HasForeignKey(x => x.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DailyClosingConfiguration : IEntityTypeConfiguration<DailyClosing>
{
    public void Configure(EntityTypeBuilder<DailyClosing> builder)
    {
        builder.ToTable("daily_closings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CashTotal).HasPrecision(12, 0);
        builder.Property(x => x.TransferTotal).HasPrecision(12, 0);
        builder.Property(x => x.CardTotal).HasPrecision(12, 0);
        builder.Property(x => x.ActualCash).HasPrecision(12, 0);
        builder.Property(x => x.Difference).HasPrecision(12, 0);
        builder.Property(x => x.ClosedByName).HasMaxLength(100);
        builder.Property(x => x.Note).HasMaxLength(255);

        builder.HasIndex(x => new { x.BranchId, x.ClosingDate }).IsUnique();

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("vouchers", t =>
        {
            t.HasCheckConstraint("CK_vouchers_value", "[value] >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.DiscountType).AsStringEnum();
        builder.Property(x => x.Value).HasPrecision(12, 0);
        builder.Property(x => x.MaxDiscount).HasPrecision(12, 0);
        builder.Property(x => x.MinOrder).HasPrecision(12, 0);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
