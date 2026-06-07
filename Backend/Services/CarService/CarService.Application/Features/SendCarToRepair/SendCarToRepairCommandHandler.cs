using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.SendCarToRepair;

public class SendCarToRepairCommandHandler(ICarRepository carRepository) : IRequestHandler<SendCarToRepairCommand>
{
    public async Task Handle(SendCarToRepairCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new InvalidOperationException($"Car with id {request.CarId} not found");

        car.SendToRepair();

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
