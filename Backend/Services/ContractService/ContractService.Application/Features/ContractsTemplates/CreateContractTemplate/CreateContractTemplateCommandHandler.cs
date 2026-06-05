using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.CreateContractTemplate;

public class CreateContractTemplateCommandHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<CreateContractTemplateCommand>
{
    public async Task Handle(CreateContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new ContractTemplate(
            request.Name,
            request.Content,
            request.ValidFrom,
            request.DocumentType,
            request.Version);

        await contractTemplateRepository.AddContractTemplateAsync(template, cancellationToken);
    }
}