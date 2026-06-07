using System.Net.Http.Headers;
using System.Net.Http.Json;
using PaymentService.Application.Abstractions.Auth;
using PaymentService.Application.Abstractions.Clients;
using PaymentService.Application.DTOs.Rentals;

namespace PaymentService.Infrastructure.Implementations.Clients
{
    public class RentalServiceClient : IRentalServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IJwtProvider _jwtProvider;

        public RentalServiceClient(HttpClient httpClient, IJwtProvider jwtProvider)
        {
            _httpClient = httpClient;
            _jwtProvider = jwtProvider;
        }

        public async Task<RentalDto> GetRentalByIdAsync(Guid rentalId)
        {
            var token = _jwtProvider.GenerateServiceToken("PaymentService", "rental.read");

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Internal/get-rental-for-payment/{rentalId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"RentalService error: {response.StatusCode}. Details: {error}");
            }

            var rental = await response.Content.ReadFromJsonAsync<RentalDto>();

            if (rental is null)
                throw new ArgumentNullException("Rental was not found.");

            return rental;
        }
    }
}
