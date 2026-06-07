using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;
using MediatR;

namespace CarService.Application.Features.ProcessReturnedCar;

public class ProcessReturnedCarCommandHandler(ICarRepository carRepository)
    : IRequestHandler<ProcessReturnedCarCommand>
{
    public async Task Handle(ProcessReturnedCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken);
        if (car is null)
            throw new InvalidOperationException($"Car with id {request.CarId} not found");

        var targetStatus = Enumeration.FromName<AvailabilityStatus>(request.TargetStatus);

        car.ProcessReturn(targetStatus);

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
