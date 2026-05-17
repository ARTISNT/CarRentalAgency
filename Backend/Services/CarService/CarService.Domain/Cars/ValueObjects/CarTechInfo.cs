using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;
using CarService.Domain.Exceptions;
using DriveType = CarService.Domain.Cars.Enums.DriveType;

namespace CarService.Domain.Cars.ValueObjects;
public record CarTechInfo : IValueObject
{
    public double Mileage { get; init; }
    public BodyStyle BodyStyle { get; init; }
    public TransmissionType TransmissionType { get; init; }
    public DriveType DriveType { get; init; }
    public EngineDetails EngineDetails { get; init; }

    public CarTechInfo(double mileage, EngineDetails engineDetails, BodyStyle bodyStyle,  
        TransmissionType transmissionType, DriveType driveType)
    {
        ValidateMileage(mileage);
        ValidateDriveType(engineDetails, transmissionType);
        
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
}
