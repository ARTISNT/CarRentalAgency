using System.Net.Http.Json;
using RentalService.Application.Common;
using RentalService.Application.Features.Rentals.CreateRental;

namespace RentalService.Infrastructure.Services.ExternalServices;

public class CarExternalService(IHttpClientFactory httpClientFactory) : ICarExternalService
{
    public async Task<CarForRentResponse> GetCarForRentAsync(Guid carId)
    {
        var client = httpClientFactory.CreateClient("CarApi");
        var car = await client.GetFromJsonAsync<CarForRentResponse>($"api/Car/get-car-for-rent/{carId}");
        
        if(car is  null)
            throw new ArgumentNullException(nameof(car));
        
        return car;
    }
}