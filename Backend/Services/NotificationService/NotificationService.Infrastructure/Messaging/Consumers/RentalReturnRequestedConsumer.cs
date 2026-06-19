using Contracts.RentalEvents;
using MassTransit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging.Consumers;

public class RentalReturnRequestedConsumer(INotificationSender sender) : IConsumer<RentalReturnRequestedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalReturnRequestedIntegrationEvent> context)
    {
        var msg = context.Message;
        await sender.SendAsync(
            msg.UserId,
            msg.UserEmail,
            NotificationType.RentalReturnRequested,
            $"Аренда {msg.RentalId}: заявка на возврат подана {msg.RequestedAt:yyyy-MM-dd HH:mm}. " +
            $"Фактическая стоимость к оплате: {msg.CostAtRequestTime:F2}");
    }
}
