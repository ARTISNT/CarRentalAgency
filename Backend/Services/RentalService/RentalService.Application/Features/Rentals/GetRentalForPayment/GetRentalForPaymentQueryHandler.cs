using MediatR;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRentalForPayment;

public class GetRentalForPaymentQueryHandler(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository)
    : IRequestHandler<GetRentalForPaymentQuery, RentalForPaymentResponse>
{
    public async Task<RentalForPaymentResponse> Handle(GetRentalForPaymentQuery request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken)
                     ?? throw new KeyNotFoundException("Rental not found");

        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken)
                      ?? throw new KeyNotFoundException("Payment not found");

        var userSnapshot = rental.CarRenterSnapshot;
        var carSnapshot = rental.RentCarSnapshot;

        return new RentalForPaymentResponse
        {
            RentalId = rental.Id,
            UserName = $"{userSnapshot.Name} {userSnapshot.SurName}",
            UserPhone = userSnapshot.PhoneNumber,
            CarName = $"{carSnapshot.Brand} {carSnapshot.Model}",
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            TotalPrice = payment.EstimatedAmount.Amount,
            DepositAmount = payment.DepositAmount.Amount
        };
    }
}
