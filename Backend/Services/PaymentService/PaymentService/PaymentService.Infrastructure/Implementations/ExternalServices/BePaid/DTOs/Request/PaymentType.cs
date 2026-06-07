using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs
{
    public class PaymentType
    {
        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = default!;
    }
}
