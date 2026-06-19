using Contracts.PaymentEvents;
using Contracts.RentalEvents;
using MediatR;
using RentalService.Application.Abstractions;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.EndRental;

public class EndRentalCommandHandler(
    IRentalRepository rentalRepository,
    IPricingPoliciesFactory pricingPoliciesFactory,
    IPaymentRepository paymentRepository,
    RentalPricingDomainService  rentalPricingDomainService,
    IIntegrationEventPublisher publisher,
    IRentalAuthorizationService authorizationService)
    : IRequestHandler<EndRentalCommand>
{
    public async Task Handle(EndRentalCommand request, CancellationToken cancellationToken)
    {
        authorizationService.EnsureCanEditRental();

        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken) ??
                     throw new KeyNotFoundException("Rental not found");
        var pricingPolicies = pricingPoliciesFactory.Create();

        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken);

        if (payment.FineOutstanding.Amount > 0)
        {
            throw new InvalidOperationException(
                "Cannot complete rental: outstanding fine must be paid first");
        }

        var currency = payment.EstimatedAmount.Currency;

        var actualCost = rentalPricingDomainService.CalculateActualCost(
            pricingPolicies, rental, request.ReturnDate, currency);
        var lateFine = rentalPricingDomainService.CalculateFine(
            pricingPolicies, rental, request.ReturnDate, currency);
        var totalCost = actualCost + lateFine;

        if (totalCost.Amount > payment.RequiredAmount.Amount)
        {
            var requiredAdditional = totalCost.Amount - payment.RequiredAmount.Amount;
            var alreadyAdded = payment.AdditionalOutstanding.Amount;
            var stillNeedsToPay = requiredAdditional - alreadyAdded;

            if (stillNeedsToPay > 0)
            {
                payment.UpdateEstimatedAmount(totalCost);
                payment.AddAdditional(
                    new Money(stillNeedsToPay, currency),
                    $"Доп. стоимость возврата ({request.ReturnDate:yyyy-MM-dd})");
                await paymentRepository.UpdatePaymentAsync(payment, cancellationToken);
                throw new InvalidOperationException(
                    $"Actual cost {totalCost.Amount:F2} {currency} exceeds required " +
                    $"{payment.RequiredAmount.Amount:F2} {currency}. " +
                    $"Already added: {alreadyAdded:F2} {currency}. " +
                    $"Client must pay additional {stillNeedsToPay:F2} {currency} before completing rental.");
            }

            payment.UpdateEstimatedAmount(totalCost);
        }

        var isEarlyReturn = request.ReturnDate < rental.EndDate;

        rental.EndRental(request.ReturnDate);

        payment.FinalizeAmount(totalCost);

        if (payment.Overpayment.Amount > 0)
        {
            payment.Refund(payment.Overpayment,
                "Early return");
        }

        if (request.PenaltyAmount > 0)
        {
            var fine = new Money(
                request.PenaltyAmount,
                payment.RequiredAmount.Currency);
            payment.AddFine(
                fine,
                string.IsNullOrWhiteSpace(request.DamageDescription)
                    ? "Penalty"
                    : request.DamageDescription);
        }

        await paymentRepository.UpdatePaymentAsync(payment, cancellationToken);
        await rentalRepository.UpdateRentalAsync(rental, cancellationToken);

        var integrationEvent = new RentalEndedIntegrationEvent(
            rental.Id,
            rental.RentCarId,
            rental.CarRenterId,
            rental.CarRenterSnapshot.Email,
            request.ReturnDate,
            payment.RequiredAmount.Amount,
            request.Mileage,
            request.FuelLevel,
            request.PenaltyAmount,
            request.DamageDescription);
        await publisher.Publish(integrationEvent, cancellationToken);

        if (request.PenaltyAmount > 0)
        {
            await publisher.Publish(new FineChargedIntegrationEvent(
                rental.Id,
                request.PenaltyAmount,
                string.IsNullOrWhiteSpace(request.DamageDescription)
                    ? "Penalty"
                    : request.DamageDescription!,
                DateTime.UtcNow), cancellationToken);
        }
    }
}