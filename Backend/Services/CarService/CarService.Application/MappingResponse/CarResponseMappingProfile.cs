using AutoMapper;
using CarService.Application.Features.GetCarForContract;
using CarService.Application.Features.GetCarForRent;
using CarService.Application.Features.GetCars;
using CarService.Application.Features.GetDetailedCars;
using CarService.Domain.Cars;

namespace CarService.Application.MappingResponse;

public class CarResponseMappingProfile : Profile
{
    public CarResponseMappingProfile()
    {
        CreateMap<Car, CarListResponse>()
            .ForMember(dest => dest.Brand,
                opt => opt.MapFrom(src => src.ModelInfo.Brand))
            .ForMember(dest => dest.Model, 
                opt => opt.MapFrom(src => src.ModelInfo.Model))
            .ForMember(dest => dest.Class,
                opt => opt.MapFrom(src => src.Class))
            .ForMember(dest => dest.PricePerHour,
                opt => opt.MapFrom(src => src.PricePerHour.Price))
            .ForMember(dest => dest.Generation,
                opt => opt.MapFrom(src => src.ModelInfo.Generation))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.IsFacelift, 
                opt => opt.MapFrom(src => src.ModelInfo.IsFacelift))
            .ForMember(dest => dest.Variant,
                opt => opt.MapFrom(src => src.ModelInfo.Variant))
            .ForMember(dest => dest.AvailabilityStatus,
                opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.LicensePlate,
                opt => opt.MapFrom(src => src.LicensePlate.Value))
            .ForMember(dest => dest.VinCode,
                opt => opt.MapFrom(src => src.VinCode.Value))
            .ForMember(dest => dest.Color,
                opt => opt.MapFrom(src => src.Color.Value))
            .ForMember(dest => dest.HorsePower,
                opt => opt.MapFrom(src => src.TechInfo.EngineDetails.HorsePower))
            .ForMember(dest => dest.EngineVolume,
                opt => opt.MapFrom(src => src.TechInfo.EngineDetails.Volume))
            .ForMember(dest => dest.PowerReverse,
                opt => opt.MapFrom(src => src.TechInfo.EngineDetails.PowerReverse))
            .ForMember(dest => dest.FuelCurrentLiters,
                opt => opt.MapFrom(src => src.TechInfo.FuelTank != null ? (double?)src.TechInfo.FuelTank.CurrentLiters : null))
            .ForMember(dest => dest.FuelCapacityLiters,
                opt => opt.MapFrom(src => src.TechInfo.FuelTank != null ? (double?)src.TechInfo.FuelTank.CapacityLiters : null))
            .ForMember(dest => dest.BatteryCurrentKWh,
                opt => opt.MapFrom(src => src.TechInfo.BatteryLevel != null ? (double?)src.TechInfo.BatteryLevel.CurrentKWh : null))
            .ForMember(dest => dest.BatteryCapacityKWh,
                opt => opt.MapFrom(src => src.TechInfo.BatteryLevel != null ? (double?)src.TechInfo.BatteryLevel.CapacityKWh : null));

        CreateMap<Car, CarDetailsResponse>()
            .IncludeBase<Car, CarListResponse>()
            .ForMember(dest => dest.DriveType,
                opt => opt.MapFrom(src => src.TechInfo.DriveType.Name))
            .ForMember(dest => dest.Transmission,
                opt => opt.MapFrom(src => src.TechInfo.TransmissionType.Name))
            .ForMember(dest => dest.Mileage,
                opt => opt.MapFrom(src => src.TechInfo.Mileage))
            .ForMember(dest => dest.VinCode,
                opt => opt.MapFrom(src => src.VinCode.Value))
            .ForMember(dest => dest.LicensePlate,
                opt => opt.MapFrom(src => src.LicensePlate.Value));
        
        CreateMap<Car, PublicCarDetailsResponse>()
            .IncludeBase<Car, CarListResponse>()
            .ForMember(dest => dest.DriveType,
                opt => opt.MapFrom(src => src.TechInfo.DriveType.Name))
            .ForMember(dest => dest.Transmission,
                opt => opt.MapFrom(src => src.TechInfo.TransmissionType.Name))
            .ForMember(dest => dest.Mileage,
                opt => opt.MapFrom(src => src.TechInfo.Mileage));

        CreateMap<Car, CarForRentResponse>()
            .ForMember(dest => dest.Model, 
                opt => opt.MapFrom(src => src.ModelInfo.Model))
            .ForMember(dest => dest.Brand,
                opt => opt.MapFrom(src => src.ModelInfo.Brand))
            .ForMember(dest => dest.Generation, 
                opt => opt.MapFrom(src => src.ModelInfo.Generation))
            .ForMember(dest => dest.Variant,
                opt => opt.MapFrom(src => src.ModelInfo.Variant))
            .ForMember(dest => dest.IsFacelift, 
                opt => opt.MapFrom(src => src.ModelInfo.IsFacelift))
            
            .ForMember(dest => dest.LicensePlate, 
                opt => opt.MapFrom(src => src.LicensePlate.Value)) 
            .ForMember(dest => dest.AvailabilityStatus, 
                opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.PricePerHour,
                opt => opt.MapFrom(src => src.PricePerHour.Price)) 
            .ForMember(dest => dest.CarClass, 
                opt => opt.MapFrom(src => src.Class.Name));

        CreateMap<Car, CarForContractResponse>()
            .ForMember(dest => dest.Model,
                opt => opt.MapFrom(src => src.ModelInfo.Model))
            .ForMember(dest => dest.Brand,
                opt => opt.MapFrom(src => src.ModelInfo.Brand))
            .ForMember(dest => dest.LicensePlate,
                opt => opt.MapFrom(src => src.LicensePlate.Value))
            .ForMember(dest => dest.Color,
                opt => opt.MapFrom(src => src.Color.Value))
            .ForMember(dest => dest.CarBodyStyle,
                opt => opt.MapFrom(src => src.TechInfo.BodyStyle.Name));
    }
}