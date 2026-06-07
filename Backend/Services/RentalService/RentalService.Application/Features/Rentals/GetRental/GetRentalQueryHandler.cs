using AutoMapper;
using MediatR;
using RentalService.Application.Authorization;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRental;

public class GetRentalQueryHandler(
    IRentalRepository rentalRepository,
    IMapper mapper,
    IRentalAuthorizationService authorizationService) : IRequestHandler<GetRentalQuery, RentalResponse>
{
    public async Task<RentalResponse> Handle(GetRentalQuery request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id) ?? 
                     throw new KeyNotFoundException($"Rental with id {request.Id} not found");

        authorizationService.EnsureCanViewRentals(rental.CarRenterId);
        
        return mapper.Map<RentalResponse>(rental);
    }
}