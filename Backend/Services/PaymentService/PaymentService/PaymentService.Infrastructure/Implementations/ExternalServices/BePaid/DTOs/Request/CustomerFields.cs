using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs
{
    public class CustomerFields
    {
        [JsonPropertyName("visible")]
        public List<string> Visible { get; set; } = default!;
    }
}
