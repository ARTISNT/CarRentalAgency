using MediatR;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.PreviewFinalCost;

public class PreviewFinalCostQueryHandler(
    IRentalRepository rentalRepository,
    IPricingPoliciesFactory pricingPoliciesFactory,
    RentalPricingDomainService rentalPricingDomainService,
    IRentalAuthorizationService authorizationService)
    : IRequestHandler<PreviewFinalCostQuery, PreviewFinalCostResponse>
{
    public async Task<PreviewFinalCostResponse> Handle(
        PreviewFinalCostQuery request,
        CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken) ??
                     throw new KeyNotFoundException("Rental not found");

        authorizationService.EnsureCanViewRentals(rental.CarRenterId);

        var pricingPolicies = pricingPoliciesFactory.Create();
        var currency = "BYN";

        var actualCost = rentalPricingDomainService.CalculateActualCost(
            pricingPolicies, rental, request.ReturnDate, currency);
        var lateFine = rentalPricingDomainService.CalculateFine(
            pricingPolicies, rental, request.ReturnDate, currency);
        var finalCost = actualCost + lateFine;

        var estimated = rentalPricingDomainService.CalculateEstimatedCost(
            pricingPolicies, rental, currency);

        var diff = finalCost.Amount - estimated.Amount;

        return new PreviewFinalCostResponse(
            finalCost.Amount,
            estimated.Amount,
            diff,
            currency);
    }
}
