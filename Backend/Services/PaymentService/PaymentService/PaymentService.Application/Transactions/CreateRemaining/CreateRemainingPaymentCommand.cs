using MediatR;

namespace PaymentService.Application.Transactions.CreateRemaining
{
    public record CreateRemainingPaymentCommand(Guid RentalId) : IRequest<string>;
}
