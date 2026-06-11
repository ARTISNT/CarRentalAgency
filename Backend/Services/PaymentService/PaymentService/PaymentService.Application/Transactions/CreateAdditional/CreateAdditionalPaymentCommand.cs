using MediatR;

namespace PaymentService.Application.Transactions.CreateAdditional
{
    public record CreateAdditionalPaymentCommand(Guid RentalId, decimal Amount, string Reason) : IRequest<string>;
}
