using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.SendCarToMaintenance;

public class SendCarToMaintenanceCommandHandler(ICarRepository carRepository) : IRequestHandler<SendCarToMaintenanceCommand>
{
    public async Task Handle(SendCarToMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new InvalidOperationException($"Car with id {request.CarId} not found");

        car.SendToMaintenance();

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
