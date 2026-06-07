using AutoMapper;
using MediatR;
using RentalService.Application.Abstractions.Security;
using RentalService.Application.Authorization;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRentals;

public class GetRentalsQueryHandler(
    IRentalRepository rentalRepository,
    IMapper mapper,
    IClientContext clientContext,
    IRentalAuthorizationPolicy authorizationPolicy) : IRequestHandler<GetRentalsQuery, IReadOnlyCollection<RentalListResponseDto>>
{
    public async Task<IReadOnlyCollection<RentalListResponseDto>> Handle(GetRentalsQuery request, CancellationToken cancellationToken)
    {
        if (!authorizationPolicy.CanViewAllRentals())
            request.RentalSpecification.CarRenterId = clientContext.ClientId;

        var rentals = await rentalRepository.GetRentalsAsync(request.RentalSpecification, cancellationToken);

        return mapper.Map<IReadOnlyCollection<RentalListResponseDto>>(rentals);
    }
}