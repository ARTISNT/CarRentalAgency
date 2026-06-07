using Contracts.RentalEvents;
using MediatR;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.RenewRental;

public class RenewRentalCommandHandler(
    IRentalRepository rentalRepository, 
    IPaymentRepository paymentRepository,
    RentalPricingDomainService pricingDomainService, 
    IPricingPoliciesFactory policiesFactory,
    IIntegrationEventPublisher publisher,
    IRentalAuthorizationService authorizationService) 
    : IRequestHandler<RenewRentalCommand>
{
    public async Task Handle(RenewRentalCommand request, CancellationToken cancellationToken)
    {
        authorizationService.EnsureCanEditRental();

        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken) ?? 
                     throw new KeyNotFoundException("Rental not found");
        
        rental.RenewRental(request.NewDate);
        var pricingPolicies = policiesFactory.Create();

        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken);
        var oldEstimatedAmount = payment.EstimatedAmount; 
        
        var newTotalCost = 
            pricingDomainService.CalculateEstimatedCost(pricingPolicies,
                rental,
                payment.EstimatedAmount.Currency);
        
        payment.UpdateEstimatedAmount(newTotalCost);

        var additionalCost = 
            newTotalCost - oldEstimatedAmount;
        
        if (additionalCost.Amount < 0)
            payment.Refund(payment.Overpayment, "");
        
        var integrationEvent = new RentalRenewedIntegrationEvent(
            rental!.Id,
            rental.RentCarId,
            rental.EndDate,
            additionalCost.Amount);

        await publisher.Publish(integrationEvent, cancellationToken);
        await rentalRepository.UpdateRentalAsync(rental, cancellationToken);
        await paymentRepository.UpdatePaymentAsync(payment, cancellationToken);
    }
}