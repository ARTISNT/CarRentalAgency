using ContractService.Application.Abstractions.Services;
using ContractService.Domain.Contracts;
using Microsoft.Extensions.Configuration;

namespace ContractService.Infrastructure.Services.ContractsGeneration;

public class ClientContractStorageManager : IContractStorage
{
    private readonly string _basePath;

    public ClientContractStorageManager(IConfiguration configuration)
    {
        _basePath = configuration["Storage:ContractsPath"]
                    ?? throw new ArgumentNullException("Storage:ContractsPath is not configured");
    }

    public string GetContractPath(Guid clientId, Contract contract)
    {
        string directory = GetClientDirectory(clientId);

        string fileName =
            $"{contract.Id}_from_{contract.Rental.StartDate:yyyy-MM-dd}_to_{contract.Rental.EndDate:yyyy-MM-dd}.pdf";

        return Path.Combine(directory, fileName);
    }

    public string GetAdditionPath(Guid clientId, Contract contract)
    {
        string directory = Path.Combine(GetClientDirectory(clientId), "additions");

        string fileName = $"{contract.Id}_addition.pdf";

        return Path.Combine(directory, fileName);
    }

    public void EnsureDirectoriesExist(Guid clientId, Contract contract)
    {
        Directory.CreateDirectory(GetClientDirectory(clientId));
        Directory.CreateDirectory(Path.Combine(GetClientDirectory(clientId), "additions"));
    }

    private string GetClientDirectory(Guid clientId)
    {
        var now = DateTime.UtcNow;

        return Path.Combine(
            _basePath,
            now.Year.ToString(),
            now.Month.ToString(),
            clientId.ToString());
    }
}
