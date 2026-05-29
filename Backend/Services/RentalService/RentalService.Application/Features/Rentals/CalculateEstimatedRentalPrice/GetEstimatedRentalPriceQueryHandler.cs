using MediatR;
using RentalService.Application.Common;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.CalculateEstimatedRentalPrice;

public class GetEstimatedRentalPriceQueryHandler(
    IRentalRepository rentalRepository,
    IPricingPoliciesFactory pricingPoliciesFactory,
    RentalPricingDomainService  rentalPricingDomainService) : IRequestHandler<GetEstimatedRentalPriceQuery, decimal>
{
    public async Task<decimal> Handle(GetEstimatedRentalPriceQuery request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.RentalId) ??
                     throw new KeyNotFoundException("Rental not found");
        var pricingPolicies = pricingPoliciesFactory.Create();

        var baseCost = rentalPricingDomainService.CalculateBaseCostWithDiscount(pricingPolicies, rental, "BYN", request.PromoCode);
        
        return baseCost.Amount;
    }
}