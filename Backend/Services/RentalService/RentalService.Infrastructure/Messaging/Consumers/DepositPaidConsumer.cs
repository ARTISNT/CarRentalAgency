using Contracts.PaymentEvents;
using MassTransit;
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
            "Processing DepositPaidEvent: Rental {RentalId}, DepositPaidAt before: {DepositPaidAt}, ContractSignedAt: {ContractSignedAt}, Status: {Status}",
            msg.RentalId, rental.DepositPaidAt, rental.ContractSignedAt, rental.ActivityStatus.Name);

        if (rental.DepositPaidAt.HasValue)
        {
            logger.LogInformation(
                "Rental {RentalId} deposit already paid at {DepositPaidAt}, skipping processing",
                msg.RentalId, rental.DepositPaidAt);
            return;
        }

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

        await AddDepositTransactionAsync(rental, msg, context.CancellationToken);

        logger.LogInformation(
            "DepositPaidEvent saved: Rental {RentalId}, DepositPaidAt: {DepositPaidAt}, ContractSignedAt: {ContractSignedAt}, Status: {Status}",
            msg.RentalId, rental.DepositPaidAt, rental.ContractSignedAt, rental.ActivityStatus.Name);

        if (!rental.ContractSignedAt.HasValue)
        {
            rental = await rentalRepository.GetRentalAsync(msg.RentalId, context.CancellationToken);
            if (rental is not null
                && rental.ContractSignedAt.HasValue
                && rental.ActivityStatus == RentActivityStatus.AwaitingConfirmation)
            {
                rental.StartRental();
                await rentalRepository.UpdateRentalAsync(rental, context.CancellationToken);

                await AddDepositTransactionAsync(rental, msg, context.CancellationToken);

                logger.LogInformation("Rental {RentalId} started after re-read (deposit consumer)", msg.RentalId);
            }
        }
    }

    private async Task AddDepositTransactionAsync(Rental rental, DepositPaidIntegrationEvent msg, CancellationToken cancellationToken)
    {
        if (!rental.PaymentId.HasValue)
        {
            logger.LogWarning("Rental {RentalId} has no PaymentId, skipping payment transaction", msg.RentalId);
            return;
        }

        var payment = await paymentRepository.GetPaymentAsync(rental.PaymentId.Value, cancellationToken);
        if (payment is null)
        {
            logger.LogWarning("Payment {PaymentId} not found for Rental {RentalId}, skipping payment transaction",
                rental.PaymentId.Value, msg.RentalId);
            return;
        }

        var externalTransactionId = $"{msg.PaymentTypeName}-{msg.RentalId:D}";

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
