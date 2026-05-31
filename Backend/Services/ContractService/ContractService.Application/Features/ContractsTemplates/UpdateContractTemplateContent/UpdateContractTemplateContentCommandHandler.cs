using ContractService.Domain.Contracts;
using ContractService.Domain.Exceptions.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.UpdateContractTemplateContent;

public class UpdateContractTemplateContentCommandHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<UpdateContractTemplateContentCommand>
{
    public async Task Handle(UpdateContractTemplateContentCommand request, CancellationToken cancellationToken)
    {
        var template = await contractTemplateRepository.GetContractTemplatesAsync(request.Id, cancellationToken)
                       ?? throw new ContractTemplateNotFoundException("Contract template not found");

        template.UpdateContent(request.Content);
        await contractTemplateRepository.UpdateContractTemplateAsync(template, cancellationToken);
    }
}