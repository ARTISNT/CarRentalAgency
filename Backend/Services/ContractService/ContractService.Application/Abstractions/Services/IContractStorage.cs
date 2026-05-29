using ContractService.Domain.Contracts;

namespace ContractService.Application.Abstractions.Services;

public interface IContractStorage
{
    string GetContractPath(Guid clientId, Contract contract);
    string GetAdditionPath(Guid clientId, Contract contract);
    void EnsureDirectoriesExist(Guid clientId, Contract contract); 
}