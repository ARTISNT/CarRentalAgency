using MediatR;

namespace PaymentService.Application.Transactions.Refund
{
    public record RefundTransactionCommand(Guid RentalId) : IRequest;
}
