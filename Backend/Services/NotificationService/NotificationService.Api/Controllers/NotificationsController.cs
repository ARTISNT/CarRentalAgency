using Contracts.UserEvents;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Notifications;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController(INotificationSender sender) : ControllerBase
{
    [HttpPost("email-verification")]
    public async Task<IActionResult> EmailVerification([FromBody] EmailVerificationRequestedIntegrationEvent request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.VerificationLink))
            return BadRequest(new { error = "invalid_request" });

        await sender.SendAsync(
            request.UserId,
            request.Email,
            NotificationType.EmailVerification,
            request.VerificationLink,
            cancellationToken);

        return Ok(new { sent = true });
    }
}
