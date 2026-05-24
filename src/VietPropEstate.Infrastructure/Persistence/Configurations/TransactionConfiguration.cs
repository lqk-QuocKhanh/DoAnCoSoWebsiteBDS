using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.ContractNumber)
            .HasMaxLength(100);

        builder.Property(t => t.Notes)
            .HasMaxLength(3000);

        builder.OwnsOne(t => t.Amount, amount =>
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

        builder.OwnsOne(t => t.CommissionAmount, commission =>
        {
            commission.Property(m => m.Amount)
                .HasColumnName("CommissionAmount")
                .HasPrecision(18, 2);

            commission.Property(m => m.Currency)
                .HasColumnName("CommissionCurrency")
                .HasMaxLength(10);
        });

        builder.HasOne(t => t.Property)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Customer)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Agent)
            .WithMany()
            .HasForeignKey(t => t.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.PropertyId);
        builder.HasIndex(t => t.CustomerId);
    }
}
