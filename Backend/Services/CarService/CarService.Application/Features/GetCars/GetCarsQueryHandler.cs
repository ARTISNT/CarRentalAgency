using AutoMapper;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetCars;

public class GetCarsQueryHandler(ICarRepository carRepository, IMapper mapper)
    : IRequestHandler<GetCarsQuery, IReadOnlyCollection<CarListResponse>>
{
    public async Task<IReadOnlyCollection<CarListResponse>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetCarsAsync(cancellationToken);
        return  mapper.Map<IReadOnlyCollection<Car>, IReadOnlyCollection<CarListResponse>>(car);
    }
}