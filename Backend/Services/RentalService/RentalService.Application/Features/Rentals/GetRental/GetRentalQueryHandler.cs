using AutoMapper;
using MediatR;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRental;

public class GetRentalQueryHandler(IRentalRepository rentalRepository, IMapper mapper) : IRequestHandler<GetRentalQuery, RentalResponse>
{
    public async Task<RentalResponse> Handle(GetRentalQuery request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id) ?? 
                     throw new KeyNotFoundException($"Rental with id {request.Id} not found");
        
        return mapper.Map<RentalResponse>(rental);
    }
}