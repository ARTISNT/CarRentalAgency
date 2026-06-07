using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.ReturnCar;

public class ReturnCarCommandHandler(ICarRepository carRepository) : IRequestHandler<ReturnCarCommand>
{
    public async Task Handle(ReturnCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new InvalidOperationException($"Car with id {request.CarId} not found");

        car.Return();

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
