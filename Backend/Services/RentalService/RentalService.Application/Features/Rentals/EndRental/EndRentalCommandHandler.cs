using Contracts.RentalEvents;
using MediatR;
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
    IIntegrationEventPublisher publisher) 
    : IRequestHandler<EndRentalCommand>
{
    public async Task Handle(EndRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken) ??
                     throw new KeyNotFoundException("Rental not found");
        var pricingPolicies = pricingPoliciesFactory.Create();
        
        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken);
        
        rental.EndRental(request.ReturnDate);
         
        var totalCost = rentalPricingDomainService.CalculateFinalCost(pricingPolicies,
            rental,
            request.ReturnDate,
            payment.EstimatedAmount.Currency);
        
        payment.FinalizeAmount(totalCost);
        
        if (payment.Overpayment.Amount > 0)
        {
            payment.Refund(payment.Overpayment,
                "Early return");
        }

        if (payment.Underpayment.Amount > 0)
        {
            
        }
        
        await rentalRepository.UpdateRentalAsync(rental, cancellationToken);
        await paymentRepository.UpdatePaymentAsync(payment, cancellationToken);

        var integrationEvent = new RentalEndedIntegrationEvent(
            rental.Id,
            rental.RentCarId,
            request.ReturnDate,
            payment.RequiredAmount.Amount);
        await publisher.Publish(integrationEvent, cancellationToken);
    }
}