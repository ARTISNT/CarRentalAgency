using Contracts.RentalEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Abstractions.UnitOfWork;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Infrastructure.Messaging.Consumers
{
    public class RentalRenewedConsumer : IConsumer<RentalRenewedIntegrationEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RentalRenewedConsumer> _logger;

        public RentalRenewedConsumer(IUnitOfWork unitOfWork, ILogger<RentalRenewedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RentalRenewedIntegrationEvent> context)
        {
            var msg = context.Message;

            if (msg.AdditionalPrice <= 0)
            {
                _logger.LogInformation(
                    "Renewal for rental {RentalId} has no additional price, skipping",
                    msg.Id);
                return;
            }

            var token = $"rental-renewal-{msg.Id}-{msg.NewEndDate:yyyyMMddHHmmss}";

            var existing = await _unitOfWork.Transactions.GetByExternalTokenAsync(token);
            if (existing is not null)
            {
                _logger.LogInformation(
                    "Renewal for rental {RentalId} already registered, skipping",
                    msg.Id);
                return;
            }

            var placeholder = new Transaction(
                msg.AdditionalPrice,
                token,
                PaymentConstants.CardId,
                msg.Id,
                PaymentType.Additional,
                $"Продление аренды до {msg.NewEndDate:yyyy-MM-dd}");

            await _unitOfWork.Transactions.CreateAsync(placeholder);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Registered additional-payment placeholder for rental {RentalId}, amount {Amount}",
                msg.Id, msg.AdditionalPrice);
        }
    }
}
