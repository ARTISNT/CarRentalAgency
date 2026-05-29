using AutoMapper;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.AddCar;

public class AddCarCommandHandler(ICarRepository carRepository, IMapper mapper) : IRequestHandler<AddCarCommand>
{
    public async Task Handle(AddCarCommand request, CancellationToken cancellationToken)
    {
        var car = Car.Create(request.ReleaseDate,
            request.LicensePlate,
            request.VinCode,
            request.Color,
            request.CarModelInfo,
            request.CarTechInfo,
            request.PricePerHour,
            request.CarClass,
            request.PhotoUrl);
        
        await carRepository.AddAsync(car, cancellationToken);
    }
}