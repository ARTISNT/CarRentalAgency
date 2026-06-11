using Contracts.ContractEvents;
using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging.Consumers;

public class ContractEndedConsumer(INotificationSender sender) : IConsumer<ContractEndedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ContractEndedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.SendAsync(
            msg.ClientId,
            null,
            NotificationType.ContractEnded,
            $"Contract {msg.ContractId}: ended on {msg.EndedAt:yyyy-MM-dd}, car {msg.CarId}, mileage {msg.Mileage} km, fuel {msg.FuelLevel}%");
    }
}
