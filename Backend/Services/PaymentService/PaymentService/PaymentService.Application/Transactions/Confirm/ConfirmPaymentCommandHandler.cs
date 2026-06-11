using Contracts.PaymentEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Application.Events;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Transactions.Confirm;

public class ConfirmPaymentCommandHandler(
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    ILogger<ConfirmPaymentCommandHandler> logger)
    : IRequestHandler<ConfirmPaymentCommand, Guid>
{
    public async Task<Guid> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var transaction = await unitOfWork.Transactions.GetByExternalTokenAsync(request.Token);

        if (transaction is null)
            throw new KeyNotFoundException("Transaction not found by token");

        var alreadyConfirmed = transaction.Status == Status.Success;
        transaction.ConfirmSuccess();

        logger.LogInformation("Transaction {TransactionId} confirmed, PaymentType: {PaymentType}",
            transaction.Id, transaction.PaymentType);

        if (alreadyConfirmed)
        {
            logger.LogInformation("Transaction {TransactionId} already confirmed, skipping event publish",
                transaction.Id);
            return transaction.RentalId;
        }

        switch (transaction.PaymentType)
        {
            case var _ when transaction.PaymentType == PaymentType.Deposit:
            case var _ when transaction.PaymentType == PaymentType.FullPayment:
                logger.LogInformation("Publishing DepositPaidIntegrationEvent for Rental {RentalId}, Type: {Type}, Amount: {Amount}",
                    transaction.RentalId, transaction.PaymentType.Name, transaction.Amount);
                await publishEndpoint.Publish(new DepositPaidIntegrationEvent(
                    transaction.RentalId, DateTime.UtcNow, transaction.PaymentType.Name, transaction.Amount), cancellationToken);
                break;

            case var _ when transaction.PaymentType == PaymentType.Fine:
                logger.LogInformation("Publishing DepositPaidIntegrationEvent for Fine on Rental {RentalId}, Amount: {Amount}",
                    transaction.RentalId, transaction.Amount);
                await publishEndpoint.Publish(new DepositPaidIntegrationEvent(
                    transaction.RentalId, DateTime.UtcNow, PaymentType.Fine.Name, transaction.Amount), cancellationToken);
                await publishEndpoint.Publish(new FinePaidIntegrationEvent(
                    transaction.RentalId, transaction.Id, transaction.Amount, DateTime.UtcNow), cancellationToken);
                break;

            case var _ when transaction.PaymentType == PaymentType.Additional:
                logger.LogInformation("Publishing DepositPaidIntegrationEvent for Additional on Rental {RentalId}, Amount: {Amount}",
                    transaction.RentalId, transaction.Amount);
                await publishEndpoint.Publish(new DepositPaidIntegrationEvent(
                    transaction.RentalId, DateTime.UtcNow, PaymentType.Additional.Name, transaction.Amount), cancellationToken);
                await publishEndpoint.Publish(new AdditionalPaidIntegrationEvent(
                    transaction.RentalId, transaction.Id, transaction.Amount, DateTime.UtcNow), cancellationToken);
                break;

            case var _ when transaction.PaymentType == PaymentType.DepositRefund:
                logger.LogInformation("Publishing DepositRefundedEvent for Rental {RentalId}",
                    transaction.RentalId);
                await publishEndpoint.Publish(new DepositRefundedEvent(
                    transaction.RentalId), cancellationToken);
                break;

            default:
                logger.LogWarning("Unexpected PaymentType {PaymentType} for Transaction {TransactionId}",
                    transaction.PaymentType, transaction.Id);
                throw new InvalidOperationException(
                    $"Unknown payment type: {transaction.PaymentType}");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction.RentalId;
    }
}
