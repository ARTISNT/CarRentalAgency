using AutoMapper;
using CarService.Application.Features.AddCar;
using CarService.Application.Features.GetCarForRent;
using CarService.Application.Features.GetCars;
using CarService.Application.Features.GetDetailedCars;
using CarService.Application.Features.UpdateCar;
using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Cars.ValueObjects;
using DriveType = CarService.Domain.Cars.Enums.DriveType;

namespace CarService.Application.Mapping;

public class CarMappingProfile : Profile
{
    public CarMappingProfile()
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
                opt => opt.MapFrom(src => src.Status.Name));

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
        
        CreateMap<CreateCarDto, Car>()
            .ConstructUsing(dto =>
                Car.Create(
                    dto.ReleaseDate,

                    new LicensePlate(dto.LicensePlate),

                    new VinCode(dto.VinCode),

                    new Color(dto.Color),

                    new CarModelInfo(
                        dto.Model,
                        dto.Brand,
                        dto.Generation,
                        dto.Variant,
                        dto.IsFacelift),

                    new CarTechInfo(
                        dto.Mileage,

                        new EngineDetails(
                            dto.HorsePower,
                            dto.EngineVolume,
                            dto.HorsePower,
                            EngineType.FromName<EngineType>(dto.EngineType)),

                        BodyStyle.FromName<BodyStyle>(dto.BodyStyle),
                        TransmissionType.FromName<TransmissionType>(dto.TransmissionType),
                        DriveType.FromName<DriveType>(dto.DriveType)),

                    new PricePerHour(dto.PricePerHour),

                    CarClass.FromName<CarClass>(dto.CarClass),

                    dto.PhotoUrl
                )); 
    }
}