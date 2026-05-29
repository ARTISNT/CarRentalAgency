using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.UpdateCar;

public class UpdateCarCommandHandler(ICarRepository carRepository) : IRequestHandler<UpdateCarCommand> 
{
    public async Task Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {

        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new KeyNotFoundException("Car not found");

        car.ChangeReleaseDate(request.ReleaseDate);
        car.ChangeLicensePlate(request.LicensePlate);
        car.Repaint(request.Color);
        car.UpdateModelInfo(request.CarModelInfo);
        car.UpdateTechInfo(request.CarTechInfo);
        car.ChangePrice(request.PricePerHour);
        car.SetCarClass(request.CarClass);
        car.ChangePhoto(request.PhotoUrl);

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}