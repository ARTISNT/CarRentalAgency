using ContractService.Application.Features.Contracts.CreateContract;

namespace ContractService.Application.Abstractions.External;

public interface IRentalExternalService
{
    public Task<RentalForContractResponse> GetRentalForContractAsync(Guid rentalId, CancellationToken cancellationToken = default);
}