using Contracts.ContractEvents;
using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging.Consumers;

public class ContractCreatedConsumer(INotificationSender sender) : IConsumer<ContractCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ContractCreatedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.SendAsync(
            msg.ClientId,
            null,
            NotificationType.ContractCreated,
            $"Contract {msg.ContractId}: created on {msg.CreatedAt:yyyy-MM-dd}, rental {msg.RentalId}");
    }
}
