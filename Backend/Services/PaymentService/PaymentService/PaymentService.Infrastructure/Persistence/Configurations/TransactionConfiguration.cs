using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Amount)
                .IsRequired();
            builder.Property(t => t.ExternalToken)
                .IsRequired();
            builder.Property(t => t.RentalId)
                .IsRequired();
            builder.Property(t => t.PaymentId)
                .IsRequired();
            builder.Property(t => t.IsRefunded)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property(t => t.PaymentDate);
            builder.Property(t => t.CreatedAt)
                .IsRequired();
            builder.Property(t => t.Description)
                .HasMaxLength(500);
            builder.Property(t => t.ExternalReceiptUrl)
                .HasMaxLength(500);
            builder.HasOne(t => t.PaymentMethod)
                .WithMany()
                .HasForeignKey(t => t.PaymentId)
                .IsRequired();
            builder.Property(t => t.Status)
                .HasConversion(
                    status => status.Value,
                    value => Status.FromValue(value));
            builder.Property(t => t.PaymentType)
                .HasConversion(
                    pt => pt.Value,
                    value => Domain.ValueObjects.PaymentType.FromValue(value));
        }
    }
}
