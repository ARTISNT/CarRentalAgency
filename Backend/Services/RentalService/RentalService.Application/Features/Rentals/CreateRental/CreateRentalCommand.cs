using Contracts.Common;
using MediatR;
using RentalService.Domain.Rentals.PricingPolicies;

namespace RentalService.Application.Features.Rentals.CreateRental;

public record CreateRentalCommand(
    Guid UserId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate,
    string? PromoCode) : IRequest<Guid>, IAuthorizedRequest;