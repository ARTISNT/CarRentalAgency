using AutoMapper;
using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using MediatR;

namespace CarService.Application.Features.GetCarForContract;

public class GetCarForContractQueryHandler(ICarRepository carRepository, IMapper mapper) : IRequestHandler<GetCarForContractQuery, CarForContractResponse>
{
    public async Task<CarForContractResponse> Handle(GetCarForContractQuery request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.Id, cancellationToken)
                  ?? throw new KeyNotFoundException("Car not found");

        if(car.Status != AvailabilityStatus.Available)
            throw new InvalidOperationException("Car is not available");
        
        return mapper.Map<CarForContractResponse>(car);
    }
}