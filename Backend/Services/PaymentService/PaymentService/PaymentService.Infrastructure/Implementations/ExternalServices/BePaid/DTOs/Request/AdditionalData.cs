using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs.Request
{
    public class AdditionalData
    {
        [JsonPropertyName("cart")]
        public Cart Cart { get; set; } = default!;
    }
}
