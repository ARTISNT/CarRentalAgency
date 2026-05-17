using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Cars.ValueObjects;
using MediatR;
using DriveType = CarService.Domain.Cars.Enums.DriveType;

namespace CarService.Application.Features.UpdateCar;

public class UpdateCarCommandHandler(ICarRepository carRepository) : IRequestHandler<UpdateCarCommand> 
{
    public async Task Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var dto = request.UpdateCarDto;

        var car = await carRepository.GetCarByIdAsync(request.CarId)
                  ?? throw new KeyNotFoundException("Car not found");

        car.ChangeReleaseDate(dto.ReleaseDate);

        car.ChangeLicensePlate(new LicensePlate(dto.LicensePlate));

        car.Repaint(new Color(dto.Color));

        car.UpdateModelInfo(new CarModelInfo(
            dto.Model,
            dto.Brand,
            dto.Generation,
            dto.Variant,
            dto.IsFacelift));

        car.UpdateTechInfo(new CarTechInfo(
            dto.Mileage,
            new EngineDetails(
                dto.HorsePower,
                dto.EngineVolume,
                null,
                EngineType.FromName<EngineType>(dto.EngineType)),
            BodyStyle.FromName<BodyStyle>(dto.BodyStyle),
            TransmissionType.FromName<TransmissionType>(dto.TransmissionType),
            DriveType.FromName<DriveType>(dto.DriveType)));

        car.ChangePrice(new PricePerHour(dto.PricePerHour));

        car.SetCarClass(CarClass.FromName<CarClass>(dto.CarClass));

        car.ChangePhoto(dto.PhotoUrl);

        await carRepository.UpdateAsync(car);
    }
}