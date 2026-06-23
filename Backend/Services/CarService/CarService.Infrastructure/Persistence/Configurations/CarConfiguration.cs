using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DriveType = CarService.Domain.Cars.Enums.DriveType;

namespace CarService.Infrastructure.Persistence.Configurations;
public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("cars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReleaseDate)
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .IsRequired();

        builder.Property(x => x.CurrentRenterId)
            .HasColumnName("current_renter_id");

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Name,
                v => Enumeration.FromName<AvailabilityStatus>(v))
            .IsRequired();

        builder.Property(x => x.Class)
            .HasConversion(
                v => v.Name,
                v => Enumeration.FromName<CarClass>(v))
            .IsRequired();
        
        builder.OwnsOne(x => x.LicensePlate,
            lp =>
            {
                lp.Property(x => x.Value)
                    .HasColumnName("license_plate")
                    .IsRequired();
                lp.HasIndex(x => x.Value).IsUnique();
            });

        builder.OwnsOne(x => x.VinCode,
            vin =>
            {
                vin.Property(x => x.Value)
                    .HasColumnName("vin_code")
                    .IsRequired();
                vin.HasIndex(x => x.Value).IsUnique();
            });

        builder.OwnsOne(x => x.Color,
            c =>
            {
                c.Property(x => x.Value)
                    .HasColumnName("color")
                    .IsRequired();
            });

        builder.OwnsOne(x => x.PricePerHour,
            p =>
            {
                p.Property(x => x.Price)
                    .HasColumnName("price_per_hour")
                    .IsRequired();
            });

        builder.OwnsOne(x => x.ModelInfo,
            mi =>
            {
                mi.Property(x => x.Brand)
                    .HasColumnName("brand");
                mi.Property(x => x.Model)
                    .HasColumnName("model");
                mi.Property(x => x.Generation)
                    .HasColumnName("generation");
                mi.Property(x => x.Variant)
                    .HasColumnName("variant");
                mi.Property(x => x.IsFacelift)
                    .HasColumnName("is_facelift");
            });

        builder.OwnsOne(x => x.TechInfo,
            ti =>
            {
                ti.Property(x => x.Mileage)
                    .HasColumnName("mileage");
                
                ti.OwnsOne(x => x.FuelTank, ft =>
                {
                    ft.Property(x => x.CurrentLiters)
                        .HasColumnName("fuel_current_liters");

                    ft.Property(x => x.CapacityLiters)
                        .HasColumnName("fuel_capacity_liters");
                });

                ti.OwnsOne(x => x.BatteryLevel, bl =>
                {
                    bl.Property(x => x.CurrentKWh)
                        .HasColumnName("battery_current_kwh");

                    bl.Property(x => x.CapacityKWh)
                        .HasColumnName("battery_capacity_kwh");
                });

                ti.Property(x => x.BodyStyle)
                    .HasConversion(
                        v => v.Name,
                        v => Enumeration.FromName<BodyStyle>(v))
                    .HasColumnName("body_style");

                ti.Property(x => x.TransmissionType)
                    .HasConversion(
                        v => v.Name,
                        v => Enumeration.FromName<TransmissionType>(v))
                    .HasColumnName("transmission");

                ti.Property(x => x.DriveType)
                    .HasConversion(
                        v => v.Name,
                        v => Enumeration.FromName<DriveType>(v))
                    .HasColumnName("drive_type");
                
                ti.OwnsOne(x => x.EngineDetails, ed =>
                {
                    ed.Property(x => x.HorsePower)
                        .HasColumnName("engine_horse_power")
                        .IsRequired();

                    ed.Property(x => x.PowerReverse)
                        .HasColumnName("engine_power_reverse");

                    ed.Property(x => x.Volume)
                        .HasColumnName("engine_volume");
                    
                    ed.Property(x => x.EngineType)
                        .HasConversion(
                            v => v.Name,
                            v => Enumeration.FromName<EngineType>(v))
                        .HasColumnName("engine_type");
                });
            });
    }
}
