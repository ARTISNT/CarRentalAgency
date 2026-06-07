using Contracts.RentalEvents;
using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging.Consumers;

public class RentalRenewedConsumer(INotificationSender sender) : IConsumer<RentalRenewedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalRenewedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.SendAsync(
            msg.UserId,
            NotificationType.RentalRenewed,
            $"Rental {msg.Id}: renewed to {msg.NewEndDate:yyyy-MM-dd}, additional cost {msg.AdditionalPrice} BYN");
    }
}
