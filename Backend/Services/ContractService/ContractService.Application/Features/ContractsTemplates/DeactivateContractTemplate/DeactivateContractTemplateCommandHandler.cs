using ContractService.Domain.Contracts;
using ContractService.Domain.Exceptions.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.DeactivateContractTemplate;

public class DeactivateContractTemplateCommandHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<DeactivateContractTemplateCommand>
{
    public async Task Handle(DeactivateContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await contractTemplateRepository.GetContractTemplatesAsync(request.Id, cancellationToken)
                       ?? throw new ContractTemplateNotFoundException("Contract template not found");

        template.Deactivate();
        await contractTemplateRepository.UpdateContractTemplateAsync(template, cancellationToken);
    }
}