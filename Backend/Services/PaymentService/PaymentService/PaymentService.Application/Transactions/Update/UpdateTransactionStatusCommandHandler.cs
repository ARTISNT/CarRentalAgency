using System.Text.Json;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Application.DTOs.PaymentGateway.Response;
using Contracts.PaymentEvents;
using PaymentService.Application.Events;
using PaymentService.Domain.ValueObjects;
using MassTransit;
using MediatR;

namespace PaymentService.Application.Transactions.Update
{
    public class UpdateTransactionStatusCommandHandler : IRequestHandler<UpdateTransactionStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateTransactionStatusCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(UpdateTransactionStatusCommand request, CancellationToken cancellationToken)
        {
            var notification = JsonSerializer.Deserialize<BePaidWebhookDto>(request.Json);

            if (notification is null)
                throw new ArgumentNullException("BePaid notification is null");

            var transaction = await _unitOfWork.Transactions.GetByRentalIdAsync(Guid.Parse(notification.Transaction.TrakingId));

            if (transaction is null)
                throw new ArgumentNullException("Transaction was not found");

            transaction.ConfirmSuccess();

            switch (transaction.PaymentType)
            {
                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.Deposit:
                    await _publishEndpoint.Publish(new DepositPaidIntegrationEvent(
                        transaction.RentalId, DateTime.UtcNow), cancellationToken);
                    break;

                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.FullPayment:
                    await _publishEndpoint.Publish(new FullPaymentPaidEvent(
                        transaction.RentalId), cancellationToken);
                    break;

                case var _ when transaction.PaymentType == Domain.ValueObjects.PaymentType.DepositRefund:
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
