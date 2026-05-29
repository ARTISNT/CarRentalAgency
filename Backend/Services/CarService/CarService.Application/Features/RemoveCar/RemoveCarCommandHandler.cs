using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.RemoveCar;

public class RemoveCarCommandHandler(ICarRepository carRepository) : IRequestHandler<RemoveCarCommand>
{
    public async Task Handle(RemoveCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken) 
            ?? throw new KeyNotFoundException("Car not found");
        
        await carRepository.DeleteAsync(car, cancellationToken);
    }
}