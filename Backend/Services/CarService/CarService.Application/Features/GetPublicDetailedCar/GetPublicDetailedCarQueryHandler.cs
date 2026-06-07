using AutoMapper;
using CarService.Application.Features.GetDetailedCars;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetPublicDetailedCar;

public class GetPublicDetailedCarQueryHandler(ICarRepository carRepository, IMapper mapper)
    : IRequestHandler<GetPublicDetailedCarQuery, PublicCarDetailsResponse>
{
    public async Task<PublicCarDetailsResponse> Handle(GetPublicDetailedCarQuery request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarByIdAsync(request.CarId, cancellationToken);
        if (car is null)
            throw new NullReferenceException("Car not found");

        return mapper.Map<PublicCarDetailsResponse>(car);
    }
}
