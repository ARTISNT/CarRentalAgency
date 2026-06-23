using AutoMapper;
using MediatR;
using RentalService.Application.Authorization;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRental;

public class GetRentalQueryHandler(
    IRentalRepository rentalRepository,
    IMapper mapper,
    IRentalAuthorizationService authorizationService,
    IPaymentRepository paymentRepository) : IRequestHandler<GetRentalQuery, RentalResponse>
{
    public async Task<RentalResponse> Handle(GetRentalQuery request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id) ??
                     throw new KeyNotFoundException($"Rental with id {request.Id} not found");

        authorizationService.EnsureCanViewRentals(rental.CarRenterId);

        var response = mapper.Map<RentalResponse>(rental);

        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken);
        if (payment != null)
        {
            response.TotalCost = payment.RequiredAmount.Amount;
            response.DepositAmount = payment.DepositAmount.Amount;
            response.PaidAmount = payment.PaidAmount.Amount;
            response.RequiredAmount = payment.RequiredAmount.Amount;
            response.RemainingAmount = payment.RemainingAmount.Amount;
            response.PaymentStatus = payment.Status.Name;
            response.Overpayment = payment.Overpayment.Amount;
            response.DepositRefund = payment.DepositAmount.Amount;
            response.FineOutstanding = payment.FineOutstanding.Amount;
            response.AdditionalOutstanding = payment.AdditionalOutstanding.Amount;
        }
        response.DepositRefundedAt = rental.DepositRefundedAt;
        return response;
    }
}