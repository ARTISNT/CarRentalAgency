using Contracts.PaymentEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Infrastructure.Messaging.Consumers
{
    public class FineChargedConsumer : IConsumer<FineChargedIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FineChargedConsumer> _logger;

        public FineChargedConsumer(IUnitOfWork unitOfWork, ILogger<FineChargedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FineChargedIntegrationEvent> context)
        {
            var msg = context.Message;

            var existing = await _unitOfWork.Transactions.GetByExternalTokenAsync(
                $"rental-fine-{msg.RentalId}");

            if (existing is not null)
            {
                _logger.LogInformation(
                    "Fine for rental {RentalId} already registered, skipping",
                    msg.RentalId);
                return;
            }

            var placeholder = new Transaction(
                msg.Amount,
                $"rental-fine-{msg.RentalId}",
                PaymentConstants.CardId,
                msg.RentalId,
                PaymentType.Fine,
                msg.Reason);

            await _unitOfWork.Transactions.CreateAsync(placeholder);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Registered fine placeholder for rental {RentalId}, amount {Amount}",
                msg.RentalId, msg.Amount);
        }
    }
}
