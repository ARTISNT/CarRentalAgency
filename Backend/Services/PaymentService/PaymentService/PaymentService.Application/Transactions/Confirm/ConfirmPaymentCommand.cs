using MediatR;

namespace PaymentService.Application.Transactions.Confirm;

public record ConfirmPaymentCommand(string Token) : IRequest<Guid>;
