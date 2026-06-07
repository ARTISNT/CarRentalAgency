using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.BreakCar;

public class BreakCarCommandHandler(ICarRepository carRepository) : IRequestHandler<BreakCarCommand>
{
    public async Task Handle(BreakCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new InvalidOperationException($"Car with id {request.CarId} not found");

        car.Break();

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
