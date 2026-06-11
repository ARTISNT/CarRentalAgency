using Contracts.RentalEvents;
using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging.Consumers;

public class RentalEndedConsumer(INotificationSender sender) : IConsumer<RentalEndedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalEndedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.SendAsync(
            msg.UserId,
            msg.UserEmail,
            NotificationType.RentalEnded,
            $"Rental {msg.RentalId}: returned on {msg.ReturnDate:yyyy-MM-dd}, total cost {msg.TotalCost} BYN");
    }
}
