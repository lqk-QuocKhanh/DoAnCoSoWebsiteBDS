using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.PaymentReference)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(100);

        builder.Property(p => p.OrderInfo)
            .HasMaxLength(500);

        builder.Property(p => p.TransactionCode)
            .HasMaxLength(500);

        builder.Property(p => p.VnpayTransactionId)
            .HasMaxLength(200);

        builder.Property(p => p.BankCode)
            .HasMaxLength(50);

        builder.Property(p => p.IpAddress)
            .HasMaxLength(45);

        builder.Property(p => p.GatewayResponse);

        builder.Property(p => p.Notes)
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.OwnsOne(p => p.Amount, amount =>
        {
            amount.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            amount.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        builder.Property(p => p.VIPPackageId);

        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.PaymentReference).IsUnique();
        builder.HasIndex(p => p.TransactionCode);
        builder.HasIndex(p => p.VIPPackageId);
        builder.HasIndex(p => p.CreatedAt);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
