using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Application.EmailOutbox;

namespace UserService.Infrastructure.EmailOutbox;

public class EmailOutboxConfiguration : IEntityTypeConfiguration<EmailOutboxEntry>
{
    public void Configure(EntityTypeBuilder<EmailOutboxEntry> builder)
    {
        builder.ToTable("email_outbox");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.VerificationLink)
            .HasColumnName("verification_link")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(x => x.Attempts)
            .HasColumnName("attempts")
            .HasDefaultValue(0);

        builder.Property(x => x.NextAttemptAt)
            .HasColumnName("next_attempt_at");

        builder.Property(x => x.LastError)
            .HasColumnName("last_error")
            .HasMaxLength(2000);

        builder.HasIndex(x => new { x.ProcessedAt, x.NextAttemptAt })
            .HasDatabaseName("IX_email_outbox_pending");
    }
}
