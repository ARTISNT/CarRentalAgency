using MediatR;
using PaymentService.Application.DTOs.Rentals;

namespace PaymentService.Application.Transactions.GetByRental
{
    public record GetPaymentSummaryQuery(Guid RentalId) : IRequest<PaymentSummaryDto>;
}
