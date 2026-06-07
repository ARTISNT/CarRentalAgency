using CarService.Domain.Common;
using CarService.Domain.Exceptions;

namespace CarService.Domain.Cars.ValueObjects;

public sealed record FuelTank : IValueObject
{
    public double CurrentLiters { get; }
    public double CapacityLiters { get; }

    public double Percentage =>
        Math.Round(CurrentLiters / CapacityLiters * 100, 2);

    public bool IsEmpty => CurrentLiters <= 0;
    public bool IsFull => CurrentLiters >= CapacityLiters;

    public FuelTank(double currentLiters, double capacityLiters)
    {
        if (capacityLiters <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacityLiters));

        if (currentLiters < 0)
            throw new ArgumentOutOfRangeException(nameof(currentLiters));

        if (currentLiters > capacityLiters)
            throw new CarDomainException(
                "Fuel level cannot exceed tank capacity.");

        CurrentLiters = currentLiters;
        CapacityLiters = capacityLiters;
    }

    public FuelTank Refill()
        => new(CapacityLiters, CapacityLiters);

    public FuelTank Consume(double liters)
    {
        if (liters <= 0)
            throw new ArgumentOutOfRangeException(nameof(liters));

        if (liters > CurrentLiters)
            throw new CarDomainException("Not enough fuel.");

        return new FuelTank(
            CurrentLiters - liters,
            CapacityLiters);
    }

    private FuelTank() { }
}