using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalService.Domain.Payments;

namespace RentalService.Infrastructure.EntitiesConfigurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RentalId)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Name,
                v => PaymentStatus.FromName<PaymentStatus>(v))
            .IsRequired();

        builder.OwnsOne(x => x.EstimatedAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("estimated_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("estimated_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(x => x.FinalAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("final_amount")
                .HasPrecision(18, 2);

            money.Property(x => x.Currency)
                .HasColumnName("final_currency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(x => x.DepositAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("deposit_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("deposit_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Metadata
            .FindNavigation(nameof(Payment.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Transactions)
            .WithOne()
            .HasForeignKey("PaymentId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}