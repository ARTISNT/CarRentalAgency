using MediatR;

namespace RentalService.Application.Features.Rentals.CalculateEstimatedRentalPrice;

public record GetEstimatedRentalPriceQuery(Guid RentalId) : IRequest<decimal>;
