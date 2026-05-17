using AutoMapper;
using MediatR;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRentals;

public class GetRentalsQueryHandler(IRentalRepository rentalRepository, IMapper mapper) : IRequestHandler<GetRentalsQuery, IReadOnlyCollection<RentalListResponseDto>>
{
    public async Task<IReadOnlyCollection<RentalListResponseDto>> Handle(GetRentalsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await rentalRepository.GetRentalsAsync();
        return mapper.Map<IReadOnlyCollection<RentalListResponseDto>>(rentals);
    }
}