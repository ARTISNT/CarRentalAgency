using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalService.Domain.Payments;

namespace RentalService.Infrastructure.EntitiesConfigurations;
public class PaymentTransactionConfiguration
    : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(
        EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("payment_transactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalTransactionId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CompletedAtUtc);

        builder.Property(x => x.Type)
            .HasConversion(
                v => v.Name,
                v => PaymentType.FromName<PaymentType>(v))
            .IsRequired();

        builder.Property(x => x.Method)
            .HasConversion(
                v => v.Name,
                v => PaymentMethod.FromName<PaymentMethod>(v))
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Name,
                v => TransactionStatus.FromName<TransactionStatus>(v))
            .IsRequired();

        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}
