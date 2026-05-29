using System.Net.Http.Headers;
using System.Net.Http.Json;
using ContractService.Application.Abstractions.External;
using ContractService.Application.Abstractions.Security;
using ContractService.Application.Features.Contracts.CreateContract;

namespace ContractService.Infrastructure.ExternalServices;

public class RentalExternalService(
    IHttpClientFactory httpClientFactory,
    IInternalJwtProvider jwtProvider) : IRentalExternalService
{
    public async Task<RentalForContractResponse> GetRentalForContractAsync(Guid rentalId, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("RentalApi");

        var token = jwtProvider.GenerateServiceToken("ContractService", "rental.read");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var rental =
            await client.GetFromJsonAsync<RentalForContractResponse>(
                $"/api/Internal/get-rental-for-contract/{rentalId}", cancellationToken);

        if (rental is null)
            throw new ArgumentNullException(nameof(rental));

        return rental;
    }
}