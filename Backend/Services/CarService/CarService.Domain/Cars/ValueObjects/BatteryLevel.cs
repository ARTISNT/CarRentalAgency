using CarService.Domain.Common;
using CarService.Domain.Exceptions;

namespace CarService.Domain.Cars.ValueObjects;

public sealed record BatteryLevel : IValueObject
{
    public double CurrentKWh { get; }
    public double CapacityKWh { get; }

    public double Percentage =>
        Math.Round(CurrentKWh / CapacityKWh * 100, 2);

    public bool IsEmpty => CurrentKWh <= 0;
    public bool IsFull => CurrentKWh >= CapacityKWh;

    public BatteryLevel(double currentKWh, double capacityKWh)
    {
        if (capacityKWh <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacityKWh));

        if (currentKWh < 0)
            throw new ArgumentOutOfRangeException(nameof(currentKWh));

        if (currentKWh > capacityKWh)
            throw new CarDomainException(
                "Battery charge cannot exceed battery capacity.");

        CurrentKWh = currentKWh;
        CapacityKWh = capacityKWh;
    }

    public BatteryLevel Recharge()
        => new(CapacityKWh, CapacityKWh);

    public BatteryLevel Consume(double kWh)
    {
        if (kWh <= 0)
            throw new ArgumentOutOfRangeException(nameof(kWh));

        if (kWh > CurrentKWh)
            throw new CarDomainException(
                "Not enough battery charge.");

        return new BatteryLevel(
            CurrentKWh - kWh,
            CapacityKWh);
    }

    private BatteryLevel() { }
}