using AutoMapper;
using CarService.Application.Authorization;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetCars;

public class GetCarsQueryHandler(
    ICarRepository carRepository,
    IMapper mapper,
    ICarAuthorizationPolicy authorizationPolicy)
    : IRequestHandler<GetCarsQuery, IReadOnlyCollection<CarListResponse>>
{
    public async Task<IReadOnlyCollection<CarListResponse>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
    {
        if (!authorizationPolicy.CanViewAllCars())
        {
            request.CarSpecification.Status = "Available";
            request.CarSpecification.DateFrom = null;
            request.CarSpecification.DateTo = null;
        }

        var cars = await carRepository.GetCarsAsync(request.CarSpecification, cancellationToken);
        return mapper.Map<IReadOnlyCollection<Car>, IReadOnlyCollection<CarListResponse>>(cars);
    }
}