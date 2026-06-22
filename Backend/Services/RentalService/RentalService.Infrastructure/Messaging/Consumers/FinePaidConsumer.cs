using Contracts.PaymentEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Infrastructure.Messaging.Consumers;

public class FinePaidConsumer(
    IPaymentRepository paymentRepository,
    ILogger<FinePaidConsumer> logger)
    : IConsumer<FinePaidIntegrationEvent>
{
    private static readonly System.Text.RegularExpressions.Regex LegacyFineIdRegex =
        new(@"^fine-[0-9a-fA-F]{32}$", System.Text.RegularExpressions.RegexOptions.Compiled);

    public async Task Consume(ConsumeContext<FinePaidIntegrationEvent> context)
    {
        var msg = context.Message;

        var payment = await paymentRepository.GetPaymentByRentIdAsync(
            msg.RentalId, context.CancellationToken);

        if (payment is null)
        {
            logger.LogWarning(
                "Payment not found for Rental {RentalId} on FinePaid event, skipping",
                msg.RentalId);
            return;
        }

        var pendingExternalId = $"fine-pending-{msg.RentalId:D}";

        var existingPending = payment.Transactions
            .FirstOrDefault(t => t.Type == PaymentType.Fine
                && t.ExternalTransactionId == pendingExternalId);

        if (existingPending is not null)
        {
            if (existingPending.Status == TransactionStatus.Completed)
            {
                logger.LogInformation(
                    "Pending fine {ExternalId} for Rental {RentalId} is already completed, skipping",
                    pendingExternalId, msg.RentalId);
                return;
            }

            if (existingPending.Status == TransactionStatus.Failed)
            {
                logger.LogWarning(
                    "Pending fine {ExternalId} for Rental {RentalId} is in Failed state, will not mark Completed",
                    pendingExternalId, msg.RentalId);
                return;
            }

            await paymentRepository.MarkPaymentTransactionCompletedAsync(
                payment.Id, pendingExternalId, context.CancellationToken);

            logger.LogInformation(
                "Marked pending fine {ExternalId} as Completed for Rental {RentalId}, Amount: {Amount}",
                pendingExternalId, msg.RentalId, msg.Amount);
            return;
        }

        var legacyPending = payment.Transactions
            .Where(t => t.Type == PaymentType.Fine
                && t.Status == TransactionStatus.Pending
                && t.ExternalTransactionId != $"fine-{msg.RentalId:D}"
                && LegacyFineIdRegex.IsMatch(t.ExternalTransactionId))
            .OrderBy(t => t.CreatedAtUtc)
            .FirstOrDefault();

        if (legacyPending is not null)
        {
            await paymentRepository.MarkPaymentTransactionCompletedAsync(
                payment.Id, legacyPending.ExternalTransactionId, context.CancellationToken);

            logger.LogInformation(
                "Marked legacy fine {ExternalId} as Completed for Rental {RentalId}, Amount: {Amount}",
                legacyPending.ExternalTransactionId, msg.RentalId, msg.Amount);
            return;
        }

        var fallbackExternalId = $"fine-{msg.RentalId:D}";
        var alreadyHasCompleted = payment.Transactions.Any(t => t.Type == PaymentType.Fine
            && t.ExternalTransactionId == fallbackExternalId
            && t.Status == TransactionStatus.Completed);

        if (alreadyHasCompleted)
        {
            logger.LogInformation(
                "Fallback fine {ExternalId} for Rental {RentalId} already exists as Completed, skipping",
                fallbackExternalId, msg.RentalId);
            return;
        }

        var amount = new Money(msg.Amount, payment.RequiredAmount.Currency);
        var transactionId = payment.AddTransaction(
            amount,
            PaymentType.Fine,
            PaymentMethod.Card,
            fallbackExternalId);
        payment.CompleteTransaction(transactionId);
        await paymentRepository.UpdatePaymentAsync(payment, context.CancellationToken);

        logger.LogInformation(
            "Created fallback fine {ExternalId} as Completed for Rental {RentalId}, Amount: {Amount}",
            fallbackExternalId, msg.RentalId, msg.Amount);
    }
}
