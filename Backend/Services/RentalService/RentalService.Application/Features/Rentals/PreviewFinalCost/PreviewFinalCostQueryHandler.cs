using MediatR;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.PreviewFinalCost;

public class PreviewFinalCostQueryHandler(
    IRentalRepository rentalRepository,
    IPricingPoliciesFactory pricingPoliciesFactory,
    RentalPricingDomainService rentalPricingDomainService,
    IPaymentRepository paymentRepository,
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

        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken);
        var depositAmount = payment?.DepositAmount.Amount ?? 0m;

        var refundAmount = diff >= 0
            ? Math.Max(0, depositAmount - diff)
            : depositAmount + (-diff);

        return new PreviewFinalCostResponse(
            finalCost.Amount,
            estimated.Amount,
            diff,
            depositAmount,
            refundAmount,
            currency);
    }
}
