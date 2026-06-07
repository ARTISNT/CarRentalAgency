using System.Text.Json.Serialization;
using PaymentService.Application.DTOs.PaymentGateway.Response.Notification;

namespace PaymentService.Application.DTOs.PaymentGateway.Response
{
    public class BePaidWebhookDto
    {
        [JsonPropertyName("transaction")]
        public BePaidTransaction Transaction { get; set; } = default!;
    }
}
