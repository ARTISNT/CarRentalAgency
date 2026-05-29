using System.Net.Http.Headers;
using System.Net.Http.Json;
using ContractService.Application.Abstractions.External;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Features.Contracts.CreateContract;

namespace ContractService.Infrastructure.ExternalServices;

public class ClientExternalService(IHttpClientFactory httpClientFactory, IInternalJwtProvider jwtProvider) : IClientExternalService
{
    public async Task<ClientForContractResponse> GetClientForRentAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var clientHttp = httpClientFactory.CreateClient("UserApi");
        
        var token = jwtProvider.GenerateServiceToken("ContractService", "User.read");
        
        clientHttp.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var client = await clientHttp.GetFromJsonAsync<ClientForContractResponse>($"/api/Internal/get-user-for-contract/{clientId}", cancellationToken);

        if(client is  null)
            throw new ArgumentNullException(nameof(client));
        
        return client;
    }
}
