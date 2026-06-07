using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs.Response
{
    public class BePaidResponse
    {
        [JsonPropertyName("checkout")]
        public Checkout Checkout { get; set; } = default!;
    }
}
