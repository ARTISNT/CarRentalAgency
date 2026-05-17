using AutoMapper;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetDetailedCars;

public class GetDetailedCarQueryHandler(ICarRepository carRepository, IMapper mapper)
    : IRequestHandler<GetDetailedCarQuery, CarDetailsResponse>
{
    public async Task<CarDetailsResponse> Handle(GetDetailedCarQuery request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken);
        if(car is null)
            throw new NullReferenceException("Car not found");
        
        return mapper.Map<CarDetailsResponse>(car);
    }
}