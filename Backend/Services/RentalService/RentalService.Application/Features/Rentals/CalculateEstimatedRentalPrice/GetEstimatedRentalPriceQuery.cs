using Contracts.Common;
using MediatR;

namespace RentalService.Application.Features.Rentals.CalculateEstimatedRentalPrice;

public record GetEstimatedRentalPriceQuery(Guid RentalId, string? PromoCode) : IRequest<decimal>, IAuthorizedRequest;
