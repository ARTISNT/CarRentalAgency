using System.Text.Json.Serialization;

namespace PaymentService.Application.DTOs.PaymentGateway.Response.Notification
{
    public class BePaidTransaction
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;
        [JsonPropertyName("tracking_id")]
        public string TrakingId { get; set; } = default!;
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
        [JsonPropertyName("receipt_url")]
        public string? ReceiptUrl { get; set; }
    }
}
