using System.Net.Http.Json;
using RentalService.Application.Abstractions;

namespace RentalService.Infrastructure.Clients;

public class PaymentServiceClient : IPaymentServiceClient
{
    private readonly HttpClient _httpClient;

    public PaymentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task RefundDepositAsync(Guid rentalId)
    {
        var response = await _httpClient.PostAsync($"/Payments/refund/{rentalId}", content: null);
        response.EnsureSuccessStatusCode();
    }
}
