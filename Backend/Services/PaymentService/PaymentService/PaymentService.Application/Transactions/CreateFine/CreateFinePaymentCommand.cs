using MediatR;

namespace PaymentService.Application.Transactions.CreateFine
{
    public record CreateFinePaymentCommand(Guid RentalId, decimal Amount, string Reason) : IRequest<string>;
}
