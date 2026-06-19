using Contracts.RentalEvents;
using MediatR;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.RequestReturnRental;

public class RequestReturnCommandHandler(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository,
    IPricingPoliciesFactory pricingPoliciesFactory,
    RentalPricingDomainService rentalPricingDomainService,
    IIntegrationEventPublisher publisher,
    IRentalAuthorizationService authorizationService)
    : IRequestHandler<RequestReturnCommand>
{
    public async Task Handle(RequestReturnCommand request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken) ??
                     throw new KeyNotFoundException("Rental not found");

        authorizationService.EnsureCanRequestReturn(rental.CarRenterId);

        var requestedAt = DateTime.UtcNow;
        rental.RequestReturn(requestedAt);

        var pricingPolicies = pricingPoliciesFactory.Create();
        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken);
        var currency = payment.EstimatedAmount.Currency;

        var actualCost = rentalPricingDomainService.CalculateActualCost(
            pricingPolicies, rental, requestedAt, currency);
        var lateFine = rentalPricingDomainService.CalculateFine(
            pricingPolicies, rental, requestedAt, currency);
        var totalCost = actualCost + lateFine;

        if (totalCost.Amount > payment.RequiredAmount.Amount)
        {
            var diff = totalCost - payment.RequiredAmount;
            payment.UpdateEstimatedAmount(totalCost);
            payment.AddAdditional(
                diff,
                $"Доплата за фактическое время на момент заявки ({requestedAt:yyyy-MM-dd HH:mm})");
            await paymentRepository.UpdatePaymentAsync(payment, cancellationToken);
        }

        await rentalRepository.UpdateRentalAsync(rental, cancellationToken);

        await publisher.Publish(
            new RentalReturnRequestedIntegrationEvent(
                rental.Id,
                rental.CarRenterId,
                rental.CarRenterSnapshot.Email,
                requestedAt,
                totalCost.Amount),
            cancellationToken);
    }
}
