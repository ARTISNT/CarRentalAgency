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
        
        builder.Property(x => x.CreatedAt).IsRequired();

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

            rental.Property(x => x.EstimatedPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
        
        // ContractAdditions (owned collection)
        builder.OwnsMany(x => x.ContractAdditions, additions =>
        {
            additions.ToTable("ContractAdditions");

            additions.WithOwner()
                .HasForeignKey("ContractId");

            additions.HasKey("Id"); 

            additions.Property(x => x.PreviousEndDate).IsRequired();
            additions.Property(x => x.NewEndDate).IsRequired();
            additions.Property(x => x.AdditionalCost)
                .HasPrecision(18, 2)
                .IsRequired();
            additions.Property(x => x.CreatedAt).IsRequired();

            // Snapshot
            additions.OwnsOne(x => x.Template, template =>
            {
                template.Property(t => t.DocumentType).IsRequired();
                template.Property(t => t.IsActive).IsRequired();
            });
        });

        // ContractReturnAct (owned 1:1)
        builder.OwnsOne(x => x.ReturnAct, act =>
        {
            act.ToTable("ContractReturnActs");

            act.Property(x => x.Mileage).IsRequired();
            act.Property(x => x.FuelLevel)
                .HasPrecision(18, 2)
                .IsRequired();
            act.Property(x => x.PenaltyAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            act.Property(x => x.DamageDescription)
                .HasMaxLength(1000);

            act.Property(x => x.CreatedAt).IsRequired();

            // Snapshot
            act.OwnsOne(x => x.Template, template =>
            {
                template.Property(t => t.DocumentType).IsRequired();
                template.Property(t => t.IsActive).IsRequired();
            });
        }); 
    }
}