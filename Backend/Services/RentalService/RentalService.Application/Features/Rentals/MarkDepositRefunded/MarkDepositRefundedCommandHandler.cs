using MediatR;
using Microsoft.Extensions.Logging;
using RentalService.Application.Authorization;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.MarkDepositRefunded;

public class MarkDepositRefundedCommandHandler(
    IRentalRepository rentalRepository,
    IRentalAuthorizationService authorizationService,
    ILogger<MarkDepositRefundedCommandHandler> logger)
    : IRequestHandler<MarkDepositRefundedCommand>
{
    public async Task Handle(MarkDepositRefundedCommand request, CancellationToken cancellationToken)
    {
        authorizationService.EnsureCanEditRental();

        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken) ??
                     throw new KeyNotFoundException("Rental not found");

        rental.MarkDepositRefundedManually(DateTime.UtcNow, request.Note);

        await rentalRepository.UpdateRentalAsync(rental, cancellationToken);

        // STUB: реальная интеграция с платёжным провайдером для возврата депозита
        // пока не подключена. Менеджер помечает возврат вручную; заглушка сигнализирует
        // о том, что фактического списания/перевода средств через PaymentService не происходит.
        logger.LogWarning(
            "STUB: deposit manually marked as refunded for rental {RentalId} (note: {Note}). " +
            "Real payment-provider integration for deposit refund is not yet implemented.",
            rental.Id, request.Note);
    }
}
