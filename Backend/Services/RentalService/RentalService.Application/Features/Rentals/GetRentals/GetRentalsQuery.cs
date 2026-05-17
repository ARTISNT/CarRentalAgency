using MediatR;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRentals;

public record GetRentalsQuery() : IRequest<IReadOnlyCollection<RentalListResponseDto>>;