using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace NotificationService.Infrastructure.Services;

public class EmailNotificationSender(
    IOptions<SmtpSettings> smtpOptions,
    ILogger<EmailNotificationSender> logger) : INotificationSender
{
    public async Task SendAsync(Guid userId, NotificationType type, string message, CancellationToken cancellationToken = default)
    {
        var settings = smtpOptions.Value;

        var subject = type switch
        {
            NotificationType.RentalCreated => "New Rental Created",
            NotificationType.RentalEnded => "Rental Ended",
            NotificationType.RentalRenewed => "Rental Renewed",
            NotificationType.ContractCreated => "New Contract Created",
            NotificationType.ContractSigned => "Contract Signed",
            NotificationType.ContractEnded => "Contract Ended",
            _ => $"Notification: {type}"
        };

        var body = $"""
            <h2>{subject}</h2>
            <p><strong>User ID:</strong> {userId}</p>
            <p><strong>Type:</strong> {type}</p>
            <hr/>
            <p>{message}</p>
            """;

        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
        mailMessage.To.Add(new MailboxAddress("", settings.ToAddress));
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(settings.Host, settings.Port, settings.UseSsl, cancellationToken);

            if (!string.IsNullOrEmpty(settings.Username))
                await client.AuthenticateAsync(settings.Username, settings.Password, cancellationToken);

            await client.SendAsync(mailMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation(
                "Email sent: To={ToAddress}, Subject={Subject}, UserId={UserId}, Type={Type}",
                settings.ToAddress, subject, userId, type);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to send email: To={ToAddress}, Subject={Subject}, UserId={UserId}, Type={Type}",
                settings.ToAddress, subject, userId, type);
            throw;
        }
    }
}

public class SmtpSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = false;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string FromAddress { get; set; } = "noreply@carrental.agency";
    public string FromName { get; set; } = "Car Rental Agency";
    public string ToAddress { get; set; } = "ag1545959@gmail.com";
}
