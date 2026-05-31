using ContractService.Application.Abstractions.Services;
using ContractService.Domain.Contracts;
using Microsoft.Extensions.Configuration;

namespace ContractService.Infrastructure.Services.ContractsGeneration;

public class ClientContractStorageManager : IContractStorage
{
    private readonly string _basePath;
    
    private const string AdditionsFolder = "additions"; 

    public ClientContractStorageManager(IConfiguration configuration)
    {
        _basePath = configuration["Storage:ContractsPath"]
                    ?? throw new ArgumentNullException("Storage:ContractsPath is not configured");
        
    }

    public string GetContractSignedPath(Guid clientId, Contract contract)
    {
        string directory = GetClientDirectory(clientId, contract);

        string fileName =
            $"{contract.Id}_from_{contract.Rental.StartDate:yyyy-MM-dd}_to_{contract.Rental.EndDate:yyyy-MM-dd}_signed.pdf";

        return Path.Combine(directory, fileName);
    }
    
    public string GetContractPath(Guid clientId, Contract contract)
    {
        string directory = GetClientDirectory(clientId, contract);

        string fileName =
            $"{contract.Id}_from_{contract.Rental.StartDate:yyyy-MM-dd}_to_{contract.Rental.EndDate:yyyy-MM-dd}.pdf";

        return Path.Combine(directory, fileName);
    }

    public string GetAdditionPath(Guid clientId, Contract contract)
    {
        string directory = Path.Combine(GetClientDirectory(clientId, contract), AdditionsFolder);

        string fileName = $"{contract.Id}_addition.pdf";

        return Path.Combine(directory, fileName);
    }

    public void EnsureDirectoriesExist(Guid clientId, Contract contract)
    {
        Directory.CreateDirectory(GetClientDirectory(clientId, contract));
        Directory.CreateDirectory(Path.Combine(GetClientDirectory(clientId, contract), AdditionsFolder));
    }

    private string GetClientDirectory(Guid clientId, Contract contract)
    {
        return Path.Combine(
            _basePath,
            contract.CreatedAt.Year.ToString(),
            contract.CreatedAt.Month.ToString(),
            clientId.ToString());
    }
}
