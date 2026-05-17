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
    RentalPricingDomainService  rentalPricingDomainService) 
    : IRequestHandler<EndRentalCommand>
{
    public async Task Handle(EndRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id) ??
                     throw new KeyNotFoundException("Rental not found");
        var pricingPolicies = pricingPoliciesFactory.Create();
        
        var totalCost = rentalPricingDomainService.CalculateTotal(pricingPolicies, rental, request.PromoCode);
        
        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id);
        payment.FinalizeAmount(new Money(totalCost, payment.DepositAmount.Currency));
        
        rental.EndRental(request.ReturnDate);
        
        await rentalRepository.UpdateRentalAsync(rental);
        await paymentRepository.UpdatePaymentAsync(payment);
    }
}