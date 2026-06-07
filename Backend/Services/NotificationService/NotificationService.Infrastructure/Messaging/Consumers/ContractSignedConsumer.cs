using Contracts.ContractEvents;
using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging.Consumers;

public class ContractSignedConsumer(INotificationSender sender) : IConsumer<ContractSignedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ContractSignedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.SendAsync(
            msg.ClientId,
            NotificationType.ContractSigned,
            $"Contract {msg.ContractId}: signed on {msg.SignedAt:yyyy-MM-dd}");
    }
}
