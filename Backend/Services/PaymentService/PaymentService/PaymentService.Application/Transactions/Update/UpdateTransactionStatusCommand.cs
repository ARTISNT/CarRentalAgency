using MediatR;

namespace PaymentService.Application.Transactions.Update
{
    public record UpdateTransactionStatusCommand(string Json) : IRequest;
}
