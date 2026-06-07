using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Abstractions;

public interface INotificationSender
{
    Task SendAsync(Guid userId, NotificationType type, string message, CancellationToken cancellationToken = default);
}
