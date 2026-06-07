using System.Text.Json.Serialization;
using PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs.Request;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs
{
    public class OrderDetails
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = default!;
        [JsonPropertyName("amount")]
        public long Amount { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("tracking_id")]
        public string TrackingId { get; set; } = default!;
        [JsonPropertyName("additional_data")]
        public AdditionalData AdditionalData { get; set; } = default!;
    }
}
