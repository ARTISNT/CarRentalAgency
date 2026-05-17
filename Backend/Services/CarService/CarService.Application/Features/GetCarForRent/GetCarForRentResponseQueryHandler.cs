using AutoMapper;
using CarService.Domain.Cars;
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
        
        return mapper.Map<CarForRentResponse>(car);
    }
}