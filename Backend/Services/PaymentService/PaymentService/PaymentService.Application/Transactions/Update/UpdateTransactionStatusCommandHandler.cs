using System.Text.Json;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Application.DTOs.PaymentGateway.Response;
using Contracts.PaymentEvents;
using PaymentService.Application.Events;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Transactions.Update
{
    public class UpdateTransactionStatusCommandHandler : IRequestHandler<UpdateTransactionStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateTransactionStatusCommandHandler> _logger;

        public UpdateTransactionStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ILogger<UpdateTransactionStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(UpdateTransactionStatusCommand request, CancellationToken cancellationToken)
        {
            var notification = JsonSerializer.Deserialize<BePaidWebhookDto>(request.Json);

            if (notification is null)
                throw new ArgumentNullException("BePaid notification is null");

            if (!string.Equals(notification.Transaction.Status, "successful", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("BePaid webhook received with non-successful status: {Status}, skipping",
                    notification.Transaction.Status);
                return;
            }

            var transaction = await _unitOfWork.Transactions.GetByRentalIdAsync(Guid.Parse(notification.Transaction.TrakingId));

            if (transaction is null)
                throw new ArgumentNullException("Transaction was not found");

            var alreadyConfirmed = transaction.Status == Status.Success;
            transaction.ConfirmSuccess(notification.Transaction.ReceiptUrl);

            if (alreadyConfirmed)
            {
                _logger.LogInformation("Transaction {TransactionId} already confirmed via webhook, skipping event publish",
                    transaction.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            switch (transaction.PaymentType)
            {
                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.Deposit:
                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.FullPayment:
                    _logger.LogInformation("Publishing DepositPaidIntegrationEvent for Rental {RentalId} via webhook, Type: {Type}, Amount: {Amount}",
                        transaction.RentalId, transaction.PaymentType.Name, transaction.Amount);
                    await _publishEndpoint.Publish(new DepositPaidIntegrationEvent(
                        transaction.RentalId, DateTime.UtcNow, transaction.PaymentType.Name, transaction.Amount), cancellationToken);
                    break;

                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.Fine:
                    _logger.LogInformation("Publishing FinePaidIntegrationEvent for Rental {RentalId} via webhook, Amount: {Amount}",
                        transaction.RentalId, transaction.Amount);
                    await _publishEndpoint.Publish(new DepositPaidIntegrationEvent(
                        transaction.RentalId, DateTime.UtcNow, Domain.ValueObjects.PaymentType.Fine.Name, transaction.Amount), cancellationToken);
                    await _publishEndpoint.Publish(new FinePaidIntegrationEvent(
                        transaction.RentalId, transaction.Id, transaction.Amount, DateTime.UtcNow), cancellationToken);
                    break;

                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.Additional:
                    _logger.LogInformation("Publishing AdditionalPaidIntegrationEvent for Rental {RentalId} via webhook, Amount: {Amount}",
                        transaction.RentalId, transaction.Amount);
                    await _publishEndpoint.Publish(new DepositPaidIntegrationEvent(
                        transaction.RentalId, DateTime.UtcNow, Domain.ValueObjects.PaymentType.Additional.Name, transaction.Amount), cancellationToken);
                    await _publishEndpoint.Publish(new AdditionalPaidIntegrationEvent(
                        transaction.RentalId, transaction.Id, transaction.Amount, DateTime.UtcNow), cancellationToken);
                    break;

                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.DepositRefund:
                    _logger.LogInformation("Publishing DepositRefundedEvent for Rental {RentalId} via webhook",
                        transaction.RentalId);
                    await _publishEndpoint.Publish(new DepositRefundedEvent(
                        transaction.RentalId), cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown payment type: {transaction.PaymentType}");
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
