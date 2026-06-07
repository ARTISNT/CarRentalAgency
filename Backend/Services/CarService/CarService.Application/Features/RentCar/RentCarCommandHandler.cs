using CarService.Application.Abstractions.Security;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.RentCar;

public class RentCarCommandHandler(ICarRepository carRepository, IClientContext clientContext)
    : IRequestHandler<RentCarCommand>
{
    public async Task Handle(RentCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken)
                  ?? throw new InvalidOperationException($"Car with id {request.CarId} not found");

        car.Rent(clientContext.ClientId);

        await carRepository.UpdateAsync(car, cancellationToken);
    }
}
