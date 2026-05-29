using System.Net.Http.Headers;
using System.Net.Http.Json;
using ContractService.Application.Abstractions.External;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Features.Contracts.CreateContract;

namespace ContractService.Infrastructure.ExternalServices;

public class CarExternalService(IHttpClientFactory httpClientFactory, IInternalJwtProvider jwtProvider) : ICarExternalService
{
    public async Task<CarForContractResponse> GetCarForContractAsync(Guid carId, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("CarApi");
        
        var token = jwtProvider.GenerateServiceToken("ContractService", "car.read");
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var car = await client.GetFromJsonAsync<CarForContractResponse>($"/api/Internal/get-car-for-contract/{carId}", cancellationToken);
        
        if(car is  null)
            throw new ArgumentNullException(nameof(car));
        
        return car;
    }
}