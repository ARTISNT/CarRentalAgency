using ContractService.Domain.Contracts;
using ContractService.Domain.Exceptions.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.ActivateContractTemplate;

public class ActivateContractTemplateCommandHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<ActivateContractTemplateCommand>
{
    public async Task Handle(ActivateContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await contractTemplateRepository.GetContractTemplatesAsync(request.Id, cancellationToken)
                       ?? throw new ContractTemplateNotFoundException("Contract template not found");

        template.Activate();
        await contractTemplateRepository.UpdateContractTemplateAsync(template, cancellationToken);
    }
}