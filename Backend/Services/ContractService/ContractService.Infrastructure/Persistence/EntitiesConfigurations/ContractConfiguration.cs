using ContractService.Domain.Common;
using ContractService.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractService.Infrastructure.Persistence.EntitiesConfigurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContractTemplateId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                status => status.Name,
                name => Enumeration.FromName<ContractStatus>(name))
            .HasMaxLength(50)
            .IsRequired();

        // ClientSnapshot
        builder.OwnsOne(x => x.Client, client =>
        {
            client.Property(x => x.PhoneNumber)
                .HasMaxLength(30)
                .IsRequired();

            client.Property(x => x.PassportIdentificationNumber)
                .HasMaxLength(50)
                .IsRequired();

            client.Property(x => x.PassportNumber)
                .HasMaxLength(30)
                .IsRequired();

            client.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            client.Property(x => x.Surname)
                .HasMaxLength(100)
                .IsRequired();

            client.Property(x => x.Patronymic)
                .HasMaxLength(100);

            client.Property(x => x.PassportIssueDate)
                .IsRequired();

            client.Property(x => x.BirthDate)
                .IsRequired();
        });

        // ContractAutoSnapshot
        builder.OwnsOne(x => x.Car, car =>
        {
            car.Property(x => x.Brand)
                .HasMaxLength(100)
                .IsRequired();

            car.Property(x => x.Model)
                .HasMaxLength(100)
                .IsRequired();

            car.Property(x => x.CarBodyStyle)
                .HasMaxLength(50)
                .IsRequired();

            car.Property(x => x.LicensePlate)
                .HasMaxLength(20)
                .IsRequired();

            car.Property(x => x.Color)
                .HasMaxLength(50)
                .IsRequired();
        });

        // ContractTemplateSnapshot
        builder.OwnsOne(x => x.Template, template =>
        {
            template.Property(x => x.Version)
                .IsRequired();

            template.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            template.Property(x => x.Content)
                .IsRequired();

            template.Property(x => x.ValidFrom)
                .IsRequired();

            template.Property(x => x.IsActive)
                .IsRequired();
        });

        // RentalSnapshot
        builder.OwnsOne(x => x.Rental, rental =>
        {
            rental.Property(x => x.StartDate)
                .IsRequired();

            rental.Property(x => x.EndDate)
                .IsRequired();

            rental.Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
    }
}