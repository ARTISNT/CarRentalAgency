using ContractService.Application.Features.Contracts.CreateContract;

namespace ContractService.Application.Abstractions.External;

public interface ICarExternalService
{
    public Task<CarForContractResponse> GetCarForContractAsync(Guid carId, CancellationToken cancellationToken = default);
}