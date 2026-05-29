using ContractService.Application.Abstractions.Services;
using ContractService.Domain.Contracts;

namespace ContractService.Application.Services;

public class ContractDocumentService(IPdfContractGenerator pdfGenerator, IContractStorage storage)
{
    public async Task GenerateContract(Guid clientId, string contractContent, Contract contract)
    {
        storage.EnsureDirectoriesExist(clientId, contract);
        
        string path = storage.GetContractPath(clientId, contract);
        
        await pdfGenerator.Generate(contractContent, path);
    }
}