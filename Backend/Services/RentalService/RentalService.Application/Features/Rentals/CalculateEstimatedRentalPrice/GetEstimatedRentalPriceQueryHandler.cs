using MediatR;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.CalculateEstimatedRentalPrice;

public class GetEstimatedRentalPriceQueryHandler(
    IRentalRepository rentalRepository,
    IPricingPoliciesFactory pricingPoliciesFactory,
    RentalPricingDomainService  rentalPricingDomainService,
    IRentalAuthorizationService authorizationService) : IRequestHandler<GetEstimatedRentalPriceQuery, decimal>
{
    public async Task<decimal> Handle(GetEstimatedRentalPriceQuery request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.RentalId, cancellationToken) ??
                     throw new KeyNotFoundException("Rental not found");

        authorizationService.EnsureCanViewRentals(rental.CarRenterId);
        
        var pricingPolicies = pricingPoliciesFactory.Create();

        var baseCost = rentalPricingDomainService.CalculateEstimatedCost(pricingPolicies, rental, "BYN");
        
        return baseCost.Amount;
    }
}