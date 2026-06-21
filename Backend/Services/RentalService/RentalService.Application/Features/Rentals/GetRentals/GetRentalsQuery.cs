using Contracts.Common;
using MediatR;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRentals;

public record GetRentalsQuery(RentalSpecification RentalSpecification) : IRequest<IReadOnlyCollection<RentalListResponseDto>>, IAuthorizedRequest;