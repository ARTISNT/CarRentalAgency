using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;

namespace CarService.Domain.Cars.ValueObjects;

public record EngineDetails : IValueObject
{
    public double HorsePower { get; }
    public double? Volume { get; }
    public double? PowerReverse { get; }
    public EngineType EngineType { get; }

    private const double MinHorsePower = 45.0;
    private const double MaxHorsePower = 900.0;

    private const double MaxEngineVolume = 4.0;
    private const double MinEngineVolume = 1.0;

    private const double MinPowerReserve = 75.0;
    private const double MaxPowerReserve = 1000.0;

    public EngineDetails(
        double horsePower,
        double? volume,
        double? powerReverse,
        EngineType engineType)
    {
        EngineType = engineType ?? throw new ArgumentNullException(nameof(engineType));

        if (horsePower is < MinHorsePower or > MaxHorsePower)
            throw new ArgumentOutOfRangeException(nameof(horsePower),
                $"Horse power must be between {MinHorsePower} and {MaxHorsePower}");

        HorsePower = horsePower;

        ValidateByEngineType(engineType, volume, powerReverse);

        Volume = volume;
        PowerReverse = powerReverse;
    }

    private void ValidateByEngineType(EngineType type, double? volume, double? powerReverse)
    {
        if (type == EngineType.Electric)
        {
            if (volume is not null)
                throw new ArgumentException("Electric engine must not have volume");

            if (powerReverse is < MinPowerReserve or > MaxPowerReserve)
                throw new ArgumentOutOfRangeException(nameof(powerReverse),
                    $"Power reserve must be between {MinPowerReserve} and {MaxPowerReserve}");

            if (powerReverse is null)
                throw new ArgumentException("Electric engine requires power reserve");

            return;
        }

        if (type == EngineType.Gasoline || type == EngineType.Diesel)
        {
            if (volume is null)
                throw new ArgumentException("ICE engines must have volume");

            if (volume is < MinEngineVolume or > MaxEngineVolume)
                throw new ArgumentOutOfRangeException(nameof(volume),
                    $"Volume must be between {MinEngineVolume} and {MaxEngineVolume}");

            return;
        }

        if (type == EngineType.HybridGasoline || type == EngineType.HybridDiesel)
        {
            if (volume is null && powerReverse is null)
                throw new ArgumentException("Hybrid engine must have either volume or power reserve");
        }
    }
}