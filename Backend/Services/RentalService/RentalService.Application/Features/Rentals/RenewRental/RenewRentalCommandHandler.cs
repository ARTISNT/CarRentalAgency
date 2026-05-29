using MediatR;
using RentalService.Application.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.RenewRental;

public class RenewRentalCommandHandler(
    IRentalRepository rentalRepository, 
    IPaymentRepository paymentRepository,
    RentalPricingDomainService pricingDomainService, 
    IPricingPoliciesFactory policiesFactory) 
    : IRequestHandler<RenewRentalCommand>
{
    public async Task Handle(RenewRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id) ?? 
                     throw new KeyNotFoundException("Rental not found");
        
        rental.RenewRental(request.NewDate);
        var pricingPolicies = policiesFactory.Create();

        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id);
        
        var newTotalCost = 
            pricingDomainService.CalculateTotal(pricingPolicies, rental, payment, 
            payment.DepositAmount.Currency);
        
        payment.UpdateEstimatedAmount(newTotalCost);
        
        await rentalRepository.UpdateRentalAsync(rental);
        await paymentRepository.UpdatePaymentAsync(payment);
    }
}