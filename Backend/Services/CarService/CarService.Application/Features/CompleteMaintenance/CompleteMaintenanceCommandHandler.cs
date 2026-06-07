using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.CompleteMaintenance;

public class CompleteMaintenanceCommandHandler(ICarRepository carRepository) : IRequestHandler<CompleteMaintenanceCommand>
{
    public async Task Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new InvalidOperationException($"Car with id {request.CarId} not found");

        car.CompleteMaintenance();

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
