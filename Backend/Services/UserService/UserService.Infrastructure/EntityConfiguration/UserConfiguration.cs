using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Roles;
using UserService.Domain.Users;

namespace UserService.Infrastructure.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{ 
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users"); 
        builder.HasKey(u => u.Id);
        builder.Ignore(u => u.DomainEvents);

        builder.OwnsOne(u => u.Email, b => { 
            b.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(Email.MaxEmailLength)
                .IsRequired(); 
            b.HasIndex(e => e.Value).IsUnique(); 
        });

        builder.OwnsOne(u => u.PhoneNumber, b => {
            b.Property(p => p.Value).HasColumnName("phone_number").IsRequired();
            b.HasIndex(p => p.Value).IsUnique();
        });

        builder.OwnsOne(u => u.Password, b => {
            b.Property(p => p.Hash).HasColumnName("password_hash").IsRequired();
        });
        
        builder.Navigation(u => u.Passport)
            .IsRequired(false);
        
        builder.OwnsOne(u => u.Passport, p => {
            p.Property(x => x.Name)
                .HasColumnName("first_name")
                .IsRequired(false)
                .HasMaxLength(Passport.MaxFullNameLength);
            p.Property(x => x.Surname)
                .HasColumnName("last_name")
                .HasMaxLength(Passport.MaxFullNameLength)
                .IsRequired(false);
            
            p.Property(x => x.Patronymic)
                .HasColumnName("patronymic")
                .HasMaxLength(Passport.MaxFullNameLength)
                .IsRequired(false);

            p.Property(x => x.BirthDate)
                .HasColumnName("birth_date");

            p.Property(x => x.PassportIssueDate)
                .HasColumnName("passport_issue_date");

            p.OwnsOne(x => x.PassportNumber, pn => {
                pn.Property(v => v.Value)
                    .HasColumnName("passport_number")
                    .IsRequired(false);
                
                pn.HasIndex(v => v.Value).IsUnique();
            });
            
            p.OwnsOne(x => x.IdentityNumber, idn => {
                idn.Property(v => v.Value)
                    .HasColumnName("identity_number")
                    .IsRequired(false);
                
                idn.HasIndex(v => v.Value).IsUnique();
            });
        });

        builder
            .Property(u => u.Role)
            .HasConversion(
                r => r.Name,
                name => Role.FromName<Role>(name)
            );

        builder
            .Property(u => u.IsActive)
            .HasColumnName("is_active");

        builder
            .Property(u => u.EmailVerified)
            .HasColumnName("email_verified");

        builder.OwnsOne(u => u.VerificationToken, v =>
        {
            v.Property(t => t.TokenHash)
                .HasColumnName("verification_token_hash")
                .HasMaxLength(256);
            v.Property(t => t.ExpiresAt)
                .HasColumnName("verification_token_expires_at");
            v.Property(t => t.CreatedAt)
                .HasColumnName("verification_token_created_at");
        });
    }
}