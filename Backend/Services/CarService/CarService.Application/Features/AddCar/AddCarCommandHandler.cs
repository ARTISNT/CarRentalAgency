using AutoMapper;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.AddCar;

public class AddCarCommandHandler(ICarRepository carRepository, IMapper mapper) : IRequestHandler<AddCarCommand>
{
    public async Task Handle(AddCarCommand request, CancellationToken cancellationToken)
    {
        var car = mapper.Map<Car>(request.CreateCarDto);
        await carRepository.AddAsync(car, cancellationToken);
    }
}