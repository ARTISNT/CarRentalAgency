using MediatR;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.CancelRental;

public class CancelRentalCommandHandler(IRentalRepository rentalRepository, IPaymentRepository paymentRepository)
    : IRequestHandler<CancelRentalCommand>
{
    public async Task Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id) ??
                     throw new KeyNotFoundException("Rental not found");
        
        rental.CancelRental(request.CancelledAt);
        
        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id);
        payment.Refund(payment.DepositAmount, request.Reason);
        
        await rentalRepository.UpdateRentalAsync(rental);
        await paymentRepository.UpdatePaymentAsync(payment);
    }
}