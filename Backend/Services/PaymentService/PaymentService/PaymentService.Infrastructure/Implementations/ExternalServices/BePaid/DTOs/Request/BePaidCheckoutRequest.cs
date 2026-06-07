using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs
{
    public class BePaidCheckoutRequest
    {
        [JsonPropertyName("checkout")]
        public CheckoutDetails Checkout { get; set; } = default!;
    }
}
