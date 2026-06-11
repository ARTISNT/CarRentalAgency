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

        var transactions = payment.Transactions
            .Select(t => new PaymentTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount.Amount,
                Type = t.Type.Name,
                Method = t.Method.Name,
                Status = t.Status.Name,
                ExternalTransactionId = t.ExternalTransactionId,
                Description = t.Description,
                CreatedAt = t.CreatedAtUtc,
                CompletedAt = t.CompletedAtUtc
            })
            .ToList();

        return new RentalForPaymentResponse
        {
            RentalId = rental.Id,
            UserName = $"{userSnapshot.Name} {userSnapshot.SurName}",
            UserPhone = userSnapshot.PhoneNumber,
            CarName = $"{carSnapshot.Brand} {carSnapshot.Model}",
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            TotalPrice = payment.RequiredAmount.Amount,
            DepositAmount = payment.DepositAmount.Amount,
            PaidAmount = payment.PaidAmount.Amount,
            RequiredAmount = payment.RequiredAmount.Amount,
            RemainingAmount = payment.RemainingAmount.Amount,
            FineOutstanding = payment.FineOutstanding.Amount,
            AdditionalOutstanding = payment.AdditionalOutstanding.Amount,
            PaymentStatus = payment.Status.Name,
            ActivityStatus = rental.ActivityStatus.Name,
            DepositPaidAt = rental.DepositPaidAt,
            Transactions = transactions
        };
    }
}
