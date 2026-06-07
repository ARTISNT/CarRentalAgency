using Contracts.RentalEvents;
using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging.Consumers;

public class RentalCreatedConsumer(INotificationSender sender) : IConsumer<RentalCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalCreatedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.SendAsync(
            msg.UserId,
            NotificationType.RentalCreated,
            $"Rental {msg.RentalId}: car {msg.CarId}, from {msg.StartDate:yyyy-MM-dd} to {msg.EndDate:yyyy-MM-dd}, estimated cost {msg.EstimatedCost} BYN");
    }
}
