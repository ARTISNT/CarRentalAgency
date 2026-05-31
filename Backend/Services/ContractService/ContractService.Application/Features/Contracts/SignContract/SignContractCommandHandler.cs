using ContractService.Application.Abstractions.Security;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using ContractService.Domain.Exceptions.Contracts;
using MediatR;

namespace ContractService.Application.Features.Contracts.SignContract;

public class SignContractCommandHandler(
    IContractRepository contractRepository,
    ContractDocumentService documentService,
    IUserContext userContext) 
    : IRequestHandler<SignContractCommand>
{
    public async Task Handle(SignContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractAsync(request.Id, cancellationToken)
            ?? throw new ContractNotFoundException("Contract not found");

        documentService.SignContract(userContext.UserId, contract);
        contract.Sign();
        
        await contractRepository.UpdateContractAsync(contract, cancellationToken);
    }
}