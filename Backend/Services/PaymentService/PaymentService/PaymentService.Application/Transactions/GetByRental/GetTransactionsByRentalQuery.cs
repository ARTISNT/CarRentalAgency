using MediatR;
using PaymentService.Application.DTOs.Rentals;

namespace PaymentService.Application.Transactions.GetByRental
{
    public record GetTransactionsByRentalQuery(Guid RentalId) : IRequest<IEnumerable<PaymentTransactionDto>>;
}
