using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs
{
    public class CheckoutDetails
    {
        [JsonPropertyName("test")]
        public bool Test { get; set; }
        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; } = "payment";
        [JsonPropertyName("attempts")]
        public int Attempts { get; set; }
        [JsonPropertyName("iframe")]
        public bool IFrame { get; set; }
        [JsonPropertyName("order")]
        public OrderDetails Order { get; set; } = default!;
        [JsonPropertyName("settings")]
        public Settings Settings { get; set; } = default!;
    }
}
