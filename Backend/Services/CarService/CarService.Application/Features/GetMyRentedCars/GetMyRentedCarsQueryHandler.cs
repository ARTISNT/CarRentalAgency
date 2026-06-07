using AutoMapper;
using CarService.Application.Abstractions.Security;
using CarService.Application.Features.GetCars;
using CarService.Domain.Cars;
using MediatR;

namespace CarService.Application.Features.GetMyRentedCars;

public class GetMyRentedCarsQueryHandler(
    ICarRepository carRepository,
    IMapper mapper,
    IClientContext clientContext)
    : IRequestHandler<GetMyRentedCarsQuery, IReadOnlyCollection<CarListResponse>>
{
    public async Task<IReadOnlyCollection<CarListResponse>> Handle(GetMyRentedCarsQuery request, CancellationToken cancellationToken)
    {
        request.Specification.Status = "Rented";
        request.Specification.RentedBy = clientContext.ClientId;

        var cars = await carRepository.GetCarsAsync(request.Specification, cancellationToken);
        return mapper.Map<IReadOnlyCollection<Car>, IReadOnlyCollection<CarListResponse>>(cars);
    }
}
