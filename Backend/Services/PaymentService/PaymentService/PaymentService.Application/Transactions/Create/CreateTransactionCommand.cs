using MediatR;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Transactions.Create
{
    public record CreateTransactionCommand(Guid RentalId, string PaymentType) : IRequest<string>;
}
