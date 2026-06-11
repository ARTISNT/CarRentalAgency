using Contracts.PaymentEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Rentals.Enums;

namespace RentalService.Infrastructure.Messaging.Consumers;

public class DepositPaidConsumer(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository,
    ILogger<DepositPaidConsumer> logger)
    : IConsumer<DepositPaidIntegrationEvent>
{
    public async Task Consume(ConsumeContext<DepositPaidIntegrationEvent> context)
    {
        var msg = context.Message;

        var rental = await rentalRepository.GetRentalAsync(msg.RentalId, context.CancellationToken);
        if (rental is null)
        {
            logger.LogWarning("Rental {RentalId} not found for deposit paid event, will retry", msg.RentalId);
            throw new Exception($"Rental {msg.RentalId} not found for deposit paid event");
        }

        logger.LogInformation(
            "Processing DepositPaidEvent: Rental {RentalId}, Type: {Type}, DepositPaidAt before: {DepositPaidAt}, ContractSignedAt: {ContractSignedAt}, Status: {Status}",
            msg.RentalId, msg.PaymentTypeName, rental.DepositPaidAt, rental.ContractSignedAt, rental.ActivityStatus.Name);

        if (msg.PaymentTypeName is "Deposit" or "FullPayment")
        {
            if (!rental.DepositPaidAt.HasValue)
            {
                rental.MarkDepositPaid(msg.PaidAt);

                if (rental.ContractSignedAt.HasValue)
                {
                    rental.StartRental();
                    logger.LogInformation("Rental {RentalId} started after deposit paid and contract signed", msg.RentalId);
                }
                else
                {
                    logger.LogInformation("Rental {RentalId} deposit paid, waiting for contract signing", msg.RentalId);
                }

                await rentalRepository.UpdateRentalAsync(rental, context.CancellationToken);
            }
            else
            {
                logger.LogInformation(
                    "Rental {RentalId} deposit already paid at {DepositPaidAt}, will still ensure transaction row exists",
                    msg.RentalId, rental.DepositPaidAt);
            }
        }
        else
        {
            logger.LogInformation(
                "Processing non-deposit event for Rental {RentalId}, Type: {Type}, Amount: {Amount}",
                msg.RentalId, msg.PaymentTypeName, msg.Amount);
        }

        var resolvedPaymentId = await ResolvePaymentIdAsync(rental, msg, context.CancellationToken);
        await AddDepositTransactionWithRetryAsync(resolvedPaymentId, msg, context.CancellationToken);

        logger.LogInformation(
            "DepositPaidEvent saved: Rental {RentalId}, Type: {Type}, DepositPaidAt: {DepositPaidAt}, ContractSignedAt: {ContractSignedAt}, Status: {Status}",
            msg.RentalId, msg.PaymentTypeName, rental.DepositPaidAt, rental.ContractSignedAt, rental.ActivityStatus.Name);

        if (!rental.ContractSignedAt.HasValue)
        {
            rental = await rentalRepository.GetRentalAsync(msg.RentalId, context.CancellationToken);
            if (rental is not null
                && rental.ContractSignedAt.HasValue
                && rental.ActivityStatus == RentActivityStatus.AwaitingConfirmation)
            {
                rental.StartRental();
                await rentalRepository.UpdateRentalAsync(rental, context.CancellationToken);

                var resolvedPaymentIdAfterReread = await ResolvePaymentIdAsync(rental, msg, context.CancellationToken);
                await AddDepositTransactionWithRetryAsync(resolvedPaymentIdAfterReread, msg, context.CancellationToken);

                logger.LogInformation("Rental {RentalId} started after re-read (deposit consumer)", msg.RentalId);
            }
        }
    }

    private async Task<Guid?> ResolvePaymentIdAsync(
        Rental rental,
        DepositPaidIntegrationEvent msg,
        CancellationToken cancellationToken)
    {
        if (rental.PaymentId.HasValue && rental.PaymentId.Value != Guid.Empty)
        {
            logger.LogInformation(
                "Resolved PaymentId {PaymentId} from rental snapshot for Rental {RentalId}",
                rental.PaymentId.Value, msg.RentalId);
            return rental.PaymentId.Value;
        }

        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken);
        if (payment is null)
        {
            logger.LogWarning(
                "Rental {RentalId} has no PaymentId on the aggregate and no Payment found by RentalId, skipping payment transaction",
                msg.RentalId);
            return null;
        }

        logger.LogInformation(
            "Resolved PaymentId {PaymentId} via Payment lookup for Rental {RentalId}",
            payment.Id, msg.RentalId);
        return payment.Id;
    }

    private async Task AddDepositTransactionWithRetryAsync(
        Guid? paymentId,
        DepositPaidIntegrationEvent msg,
        CancellationToken cancellationToken)
    {
        if (!paymentId.HasValue)
        {
            logger.LogWarning("Rental {RentalId} has no PaymentId, skipping payment transaction", msg.RentalId);
            return;
        }

        const int maxAttempts = 5;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await AddDepositTransactionAsync(paymentId.Value, msg, cancellationToken);
                return;
            }
            catch (DbUpdateConcurrencyException) when (attempt < maxAttempts)
            {
                var delay = TimeSpan.FromMilliseconds(100 * attempt);
                logger.LogWarning(
                    "Concurrency conflict on Payment {PaymentId} for Rental {RentalId} (attempt {Attempt}/{MaxAttempts}), retrying in {Delay}ms",
                    paymentId.Value, msg.RentalId, attempt, maxAttempts, delay.TotalMilliseconds);
                await Task.Delay(delay, cancellationToken);
            }
            catch (DbUpdateException ex) when (attempt < maxAttempts)
            {
                var delay = TimeSpan.FromMilliseconds(100 * attempt);
                logger.LogWarning(ex,
                    "DbUpdateException on Payment {PaymentId} for Rental {RentalId} (attempt {Attempt}/{MaxAttempts}): {Message}. Retrying in {Delay}ms",
                    paymentId.Value, msg.RentalId, attempt, maxAttempts, ex.InnerException?.Message ?? ex.Message, delay.TotalMilliseconds);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private async Task AddDepositTransactionAsync(
        Guid paymentId,
        DepositPaidIntegrationEvent msg,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetPaymentAsync(paymentId, cancellationToken);
        if (payment is null)
        {
            logger.LogWarning("Payment {PaymentId} not found for Rental {RentalId}, skipping payment transaction",
                paymentId, msg.RentalId);
            return;
        }

        var externalTransactionId = msg.PaymentTypeName switch
        {
            "Fine" => $"fine-{msg.RentalId:D}",
            "Additional" => $"renewal-{msg.RentalId:D}",
            "FullPayment" => $"fullpayment-{msg.RentalId:D}",
            _ => $"deposit-{msg.RentalId:D}",
        };

        if (payment.Transactions.Any(t => t.ExternalTransactionId == externalTransactionId))
        {
            logger.LogInformation(
                "Payment {PaymentId} already has transaction with ExternalTransactionId {ExternalTransactionId} for Rental {RentalId}, skipping",
                payment.Id, externalTransactionId, msg.RentalId);
            return;
        }

        var (sourceAmount, type) = msg.PaymentTypeName switch
        {
            "FullPayment" => (payment.RequiredAmount, PaymentType.FinalPayment),
            "Fine" => (new Money(msg.Amount, payment.RequiredAmount.Currency), PaymentType.Fine),
            "Additional" => (new Money(msg.Amount, payment.RequiredAmount.Currency), PaymentType.Additional),
            _ => (payment.DepositAmount, PaymentType.Deposit),
        };

        var amount = new Money(sourceAmount.Amount, sourceAmount.Currency);

        var transactionId = payment.AddTransaction(
            amount,
            type,
            PaymentMethod.Card,
            externalTransactionId);

        payment.CompleteTransaction(transactionId);
        await paymentRepository.UpdatePaymentAsync(payment, cancellationToken);

        logger.LogInformation(
            "Payment {PaymentId} updated with {PaymentTypeName} transaction for Rental {RentalId}, PaidAmount now: {PaidAmount}",
            payment.Id, msg.PaymentTypeName, msg.RentalId, payment.PaidAmount.Amount);
    }
}
