using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.MarkCarAsReturned;

public class MarkCarAsReturnedCommandHandler(ICarRepository carRepository) : IRequestHandler<MarkCarAsReturnedCommand>
{
    public async Task Handle(MarkCarAsReturnedCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new InvalidOperationException($"Car with id {request.CarId} not found");

        car.MarkAsReturned();

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
