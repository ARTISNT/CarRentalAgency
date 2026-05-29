using AutoMapper;
using CarService.Api.Requests;
using CarService.Application.Features.AddCar;
using CarService.Application.Features.UpdateCar;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Cars.ValueObjects;
using DriveType = CarService.Domain.Cars.Enums.DriveType;

namespace CarService.Api.MappingRequests;

public class CarMappingRequests : Profile
{
    public CarMappingRequests()
    {
        CreateMap<CreateCarRequest, AddCarCommand>()
            .ForCtorParam(nameof(AddCarCommand.LicensePlate),
                opt => opt.MapFrom(src => new LicensePlate(src.LicensePlate)))
            .ForCtorParam(nameof(AddCarCommand.VinCode),
                opt => opt.MapFrom(src => new VinCode(src.VinCode)))
            .ForCtorParam(nameof(AddCarCommand.Color),
                opt => opt.MapFrom(src => new Color(src.Color)))
            .ForCtorParam(nameof(AddCarCommand.CarClass),
                opt => opt.MapFrom(src => CarClass.FromName<CarClass>(src.CarClass)))
            .ForCtorParam(nameof(AddCarCommand.PricePerHour),
                opt => opt.MapFrom(src => new PricePerHour(src.PricePerHour)))
            .ForCtorParam(nameof(AddCarCommand.CarModelInfo), opt => opt.MapFrom(src =>
                new CarModelInfo(src.Model, src.Brand, src.Generation, src.Variant, src.IsFacelift)))
            .ForCtorParam(nameof(AddCarCommand.CarTechInfo), opt => opt.MapFrom(src =>
                new CarTechInfo(
                    src.Mileage,
                    new EngineDetails(src.HorsePower, src.EngineVolume, src.PowerReverse,
                        EngineType.FromName<EngineType>(src.EngineType)),
                    BodyStyle.FromName<BodyStyle>(src.BodyStyle),
                    TransmissionType.FromName<TransmissionType>(src.TransmissionType),
                    DriveType.FromName<DriveType>(src.DriveType)
                )));

        CreateMap<(Guid Id, UpdateCarRequests Dto), UpdateCarCommand>()
            .ForCtorParam(nameof(UpdateCarCommand.CarId),
                opt => opt.MapFrom(src => src.Id))

            .ForCtorParam(nameof(UpdateCarCommand.ReleaseDate), opt => opt.MapFrom(src => src.Dto.ReleaseDate))
            .ForCtorParam(nameof(UpdateCarCommand.LicensePlate),
                opt => opt.MapFrom(src => new LicensePlate(src.Dto.LicensePlate)))
            .ForCtorParam(nameof(UpdateCarCommand.VinCode), opt => opt.MapFrom(src => new VinCode(src.Dto.VinCode)))
            .ForCtorParam(nameof(UpdateCarCommand.Color), opt => opt.MapFrom(src => new Color(src.Dto.Color)))
            .ForCtorParam(nameof(UpdateCarCommand.PricePerHour),
                opt => opt.MapFrom(src => new PricePerHour(src.Dto.PricePerHour)))
            .ForCtorParam(nameof(UpdateCarCommand.CarClass),
                opt => opt.MapFrom(src => CarClass.FromName<CarClass>(src.Dto.CarClass)))
            .ForCtorParam(nameof(UpdateCarCommand.PhotoUrl), opt => opt.MapFrom(src => src.Dto.PhotoUrl))

            .ForCtorParam(nameof(UpdateCarCommand.CarModelInfo), opt => opt.MapFrom(src =>
                new CarModelInfo(src.Dto.Model, src.Dto.Brand, src.Dto.Generation, src.Dto.Variant,
                    src.Dto.IsFacelift)))
            .ForCtorParam(nameof(UpdateCarCommand.CarTechInfo), opt => opt.MapFrom(src =>
                new CarTechInfo(
                    src.Dto.Mileage,
                    new EngineDetails(src.Dto.HorsePower, src.Dto.EngineVolume, src.Dto.PowerReverse,
                        EngineType.FromName<EngineType>(src.Dto.EngineType)), 
                    BodyStyle.FromName<BodyStyle>(src.Dto.BodyStyle),
                    TransmissionType.FromName<TransmissionType>(src.Dto.TransmissionType),
                    DriveType.FromName<DriveType>(src.Dto.DriveType)
                )));
    }
}