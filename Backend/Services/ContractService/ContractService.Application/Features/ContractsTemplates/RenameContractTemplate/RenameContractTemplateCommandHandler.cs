using ContractService.Application.Exceptions.Contracts;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.RenameContractTemplate;

public class RenameContractTemplateCommandHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<RenameContractTemplateCommand>
{
    public async Task Handle(RenameContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await contractTemplateRepository.GetContractTemplatesAsync(request.Id, cancellationToken)
                       ?? throw new ContractTemplateNotFoundException("Contract template not found");

        template.Rename(request.Name);
        await contractTemplateRepository.UpdateContractTemplateAsync(template, cancellationToken);
    }
}