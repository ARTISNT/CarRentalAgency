using System.Net.Http.Headers;
using System.Net.Http.Json;
using RentalService.Application.Common;
using RentalService.Application.Features.Rentals.CreateRental;

namespace RentalService.Infrastructure.Services.ExternalServices;

public class UserExternalService(IHttpClientFactory httpClientFactory, IJwtProvider jwtProvider) : IUserExternalService
{
    public async Task<UserRentInfoResponse> GetUserForRentAsync(Guid userId)
    {
        var client = httpClientFactory.CreateClient("UserApi");
        
        var token = jwtProvider.GenerateServiceToken("RentalService", "user.read");
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var user = await client.GetFromJsonAsync<UserRentInfoResponse>($"/api/InternalUser/get-user-for-rent/{userId}");

        if(user is  null)
            throw new ArgumentNullException(nameof(user));
        
        return user;
    }
}