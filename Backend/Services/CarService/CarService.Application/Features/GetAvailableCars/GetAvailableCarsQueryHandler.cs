using AutoMapper;
using CarService.Application.Features.GetCars;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetAvailableCars;

public class GetAvailableCarsQueryHandler(ICarRepository carRepository, IMapper mapper)
    : IRequestHandler<GetAvailableCarsQuery, IReadOnlyCollection<CarListResponse>>
{
    public async Task<IReadOnlyCollection<CarListResponse>> Handle(GetAvailableCarsQuery request, CancellationToken cancellationToken)
    {
        request.Specification.Status = "Available";
        request.Specification.DateFrom = null;
        request.Specification.DateTo = null;

        var cars = await carRepository.GetCarsAsync(request.Specification, cancellationToken);
        return mapper.Map<IReadOnlyCollection<Car>, IReadOnlyCollection<CarListResponse>>(cars);
    }
}
