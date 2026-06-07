using ContractService.Domain.Contracts;

namespace ContractService.Application.Abstractions.Services;

public interface IContractStorage
{
    string GetContractSignedPath(Guid clientId, Contract contract);
    string GetContractPath(Guid clientId, Contract contract);
    string GetAdditionPath(Guid clientId, Contract contract);
    string GetReturnActPath(Guid clientId, Contract contract);
    void EnsureDirectoriesExist(Guid clientId, Contract contract); 
}