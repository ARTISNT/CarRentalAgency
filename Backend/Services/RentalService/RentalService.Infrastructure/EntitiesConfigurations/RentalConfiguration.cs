using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalService.Domain.Rentals;
using RentalService.Domain.Rentals.Enums;

namespace RentalService.Infrastructure.EntitiesConfigurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("Rentals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.ReturnDate);
        builder.Property(x => x.ReturnRequestedAtUtc);

        builder.Property(x => x.ActivityStatus)
            .HasConversion(
                v => v.Name, 
                v => RentActivityStatus.FromName(v)) 
            .IsRequired();

        builder.OwnsOne(x => x.RentCarSnapshot, snapshot =>
        {
            snapshot.Property(p => p.Model).IsRequired().HasColumnName("Car_Model");
            snapshot.Property(p => p.Brand).IsRequired().HasColumnName("Car_Brand");
            snapshot.Property(p => p.Generation).HasColumnName("Car_Generation");
            snapshot.Property(p => p.Variant).HasColumnName("Car_Variant");
            snapshot.Property(p => p.IsFacelift).IsRequired().HasColumnName("Car_IsFacelift");
            snapshot.Property(p => p.LicensePlate).IsRequired().HasColumnName("Car_LicensePlate");
            snapshot.Property(p => p.PricePerHour).IsRequired().HasColumnName("Car_PricePerHour").HasPrecision(18, 2);
            snapshot.Property(p => p.AvailabilityStatus).IsRequired().HasColumnName("Car_AvailabilityStatus");
            snapshot.Property(p => p.CarClass).IsRequired().HasColumnName("Car_CarClass");

            snapshot.WithOwner();
        });

        builder.OwnsOne(x => x.CarRenterSnapshot, snapshot =>
        {
            snapshot.Property(p => p.Name).IsRequired().HasColumnName("Renter_Name");
            snapshot.Property(p => p.SurName).IsRequired().HasColumnName("Renter_SurName");
            snapshot.Property(p => p.PhoneNumber).IsRequired().HasColumnName("Renter_PhoneNumber");
            snapshot.Property(p => p.Patronymic).IsRequired().HasColumnName("Renter_Patronymic");
            snapshot.Property(p => p.Email).IsRequired().HasColumnName("Renter_Email");

            snapshot.WithOwner();
        });

        builder.Ignore("DomainEvents");
    }
}