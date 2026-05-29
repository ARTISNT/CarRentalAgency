using ContractService.Domain.Contracts;

namespace ContractService.Application.Abstractions.Services;

public interface IPdfContractGenerator
{
    public Task Generate(string html, string outputPath);
}