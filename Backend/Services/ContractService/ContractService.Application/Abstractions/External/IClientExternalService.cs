using ContractService.Application.Features.Contracts.CreateContract;

namespace ContractService.Application.Abstractions.External;

public interface IClientExternalService
{
    public Task<ClientForContractResponse> GetClientForRentAsync(Guid clientId,
        CancellationToken cancellationToken = default);
}