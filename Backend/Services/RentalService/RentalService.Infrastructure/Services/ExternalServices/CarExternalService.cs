using System.Net.Http.Headers;
using System.Net.Http.Json;
using RentalService.Application.Common;
using RentalService.Application.Features.Rentals.CreateRental;

namespace RentalService.Infrastructure.Services.ExternalServices;

public class CarExternalService(IHttpClientFactory httpClientFactory, IJwtProvider jwtProvider) : ICarExternalService
{
    public async Task<CarForRentResponse> GetCarForRentAsync(Guid carId)
    {
        var client = httpClientFactory.CreateClient("CarApi");
        
        var token = jwtProvider.GenerateServiceToken("RentalService", "car.read");
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var car = await client.GetFromJsonAsync<CarForRentResponse>($"/api/Internal/get-car-for-rent/{carId}");
        
        if(car is  null)
            throw new ArgumentNullException(nameof(car));
        
        return car;
    }
}