using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs
{
    public class Customer
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = default!;
        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = default!;
        [JsonPropertyName("address")]
        public string Address { get; set; } = default!;
        [JsonPropertyName("country")]
        public string Country { get; set; } = default!;
        [JsonPropertyName("city")]
        public string City { get; set; } = default!;
    }
}
