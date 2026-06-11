using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Services;

public class ConsoleNotificationSender(ILogger<ConsoleNotificationSender> logger) : INotificationSender
{
    public Task SendAsync(Guid userId, string? email, NotificationType type, string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[NOTIFICATION] User: {UserId}, Email: {Email}, Type: {NotificationType}, Message: {Message}",
            userId, email, type, message);
        return Task.CompletedTask;
    }
}