using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs.Response
{
    public class Checkout
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = default!;
        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; } = default!;
    }
}
