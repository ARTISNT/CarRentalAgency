using AutoMapper;
using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using MediatR;

namespace CarService.Application.Features.GetCarForRent;

public class GetCarForRentResponseQueryHandler(ICarRepository carRepository, IMapper mapper)
    : IRequestHandler<GetCarForRentQuery, CarForRentResponse>
{
    public async Task<CarForRentResponse> Handle(GetCarForRentQuery request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.Id, cancellationToken);
        if(car is null)
            throw new NullReferenceException("Car not found");
        
        if(car.Status != AvailabilityStatus.Available)
            throw new InvalidOperationException("Car is not available");
        
        return mapper.Map<CarForRentResponse>(car);
    }
}