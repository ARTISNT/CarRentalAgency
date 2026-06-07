using ContractService.Application.Abstractions.Services;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContractPdf;

public class GetContractPdfQueryHandler(
    IContractRepository contractRepository,
    IContractStorage contractStorage)
    : IRequestHandler<GetContractPdfQuery, ContractPdfResponse>
{
    public async Task<ContractPdfResponse> Handle(GetContractPdfQuery request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractAsync(request.ContractId, cancellationToken)
                       ?? throw new ContractNotFoundException("Contract not found");

        var filePath = request.Signed
            ? contractStorage.GetContractSignedPath(contract.ClientId, contract)
            : contractStorage.GetContractPath(contract.ClientId, contract);

        var exists = File.Exists(filePath);

        return new ContractPdfResponse
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            ContentType = "application/pdf",
            Exists = exists
        };
    }
}