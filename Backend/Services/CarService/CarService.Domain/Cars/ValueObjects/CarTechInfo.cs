using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;
using CarService.Domain.Exceptions;
using DriveType = CarService.Domain.Cars.Enums.DriveType;

namespace CarService.Domain.Cars.ValueObjects;
public record CarTechInfo : IValueObject
{
    public double Mileage { get; init; }
    public FuelTank? FuelTank { get; init; }
    public BatteryLevel? BatteryLevel { get; init; }
    public BodyStyle BodyStyle { get; init; }
    public TransmissionType TransmissionType { get; init; }
    public DriveType DriveType { get; init; }
    public EngineDetails EngineDetails { get; init; }

    public CarTechInfo(FuelTank? fuelTank, BatteryLevel? batteryLevel, double mileage, EngineDetails engineDetails,
        BodyStyle bodyStyle,
        TransmissionType transmissionType, DriveType driveType)
    {
        ValidateMileage(mileage);
        ValidateDriveType(engineDetails, transmissionType);
        ValidateEnergyStorage(engineDetails, fuelTank, batteryLevel);
        
        FuelTank = fuelTank;
        BatteryLevel = batteryLevel;
        Mileage = mileage;
        TransmissionType = transmissionType;
        DriveType = driveType;
        BodyStyle = bodyStyle;
        EngineDetails = engineDetails;
    }

    protected CarTechInfo(){}
    
    private void ValidateMileage(double mileage)
    {
        if (mileage < 0) 
            throw new ArgumentOutOfRangeException(nameof(mileage));
    }

    private void ValidateDriveType(EngineDetails engineDetails, TransmissionType transmissionType)
    {
        if (engineDetails.EngineType == EngineType.Electric || 
            engineDetails.EngineType == EngineType.HybridDiesel || 
            engineDetails.EngineType == EngineType.HybridGasoline && 
            Equals(transmissionType, TransmissionType.Manual))
            
            throw new CarDomainException("Electric/Hybrid car cant have manual transmission");
    }

    private void ValidateEnergyStorage(
        EngineDetails engineDetails,
        FuelTank? fuelTank,
        BatteryLevel? batteryLevel)
    {
        if (engineDetails.EngineType.Equals(EngineType.Electric))
        {
            if (batteryLevel is null)
                throw new CarDomainException(
                    "Electric vehicle must have a battery.");

            if (fuelTank is not null)
                throw new CarDomainException(
                    "Electric vehicle cannot have a fuel tank.");
        }

        if (engineDetails.EngineType.Equals(EngineType.Gasoline) ||
            engineDetails.EngineType.Equals(EngineType.Diesel))
        {
            if (fuelTank is null)
                throw new CarDomainException(
                    "Combustion vehicle must have a fuel tank.");

            if (batteryLevel is not null)
                throw new CarDomainException(
                    "Combustion vehicle cannot have a battery.");
        }

        if (engineDetails.EngineType.Equals(EngineType.HybridGasoline) ||
            engineDetails.EngineType.Equals(EngineType.HybridDiesel))
        {
            if (fuelTank is null || batteryLevel is null)
                throw new CarDomainException(
                    "Hybrid vehicle must have both fuel tank and battery.");
        }
    }
}