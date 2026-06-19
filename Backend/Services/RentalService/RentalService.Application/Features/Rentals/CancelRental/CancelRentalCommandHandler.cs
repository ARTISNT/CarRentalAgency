using MediatR;
using RentalService.Application.Authorization;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.CancelRental;

public class CancelRentalCommandHandler(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository,
    IRentalAuthorizationService authorizationService)
    : IRequestHandler<CancelRentalCommand>
{
    public async Task Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        authorizationService.EnsureCanEditRental();

        var rental = await rentalRepository.GetRentalAsync(request.Id) ??
                     throw new KeyNotFoundException("Rental not found");
        
        rental.CancelRental(request.CancelledAt, request.Reason);
        
        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id);
        if (payment.PaidAmount.Amount > 0)
            payment.Refund(payment.PaidAmount, request.Reason);
        
        await paymentRepository.UpdatePaymentAsync(payment);
        await rentalRepository.UpdateRentalAsync(rental);
    }
}