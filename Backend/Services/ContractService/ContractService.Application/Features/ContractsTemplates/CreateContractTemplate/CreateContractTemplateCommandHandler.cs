using ContractService.Domain.Common;
using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.CreateContractTemplate;

public class CreateContractTemplateCommandHandler(
    IContractTemplateRepository contractTemplateRepository)
    : IRequestHandler<CreateContractTemplateCommand>
{
    public async Task Handle(CreateContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var documentType = Enumeration.FromName<DocumentType>(request.DocumentType);
        var validFrom = request.ValidFrom ?? DateTime.UtcNow;

        var template = new ContractTemplate(
            request.Name,
            request.Content,
            validFrom,
            documentType,
            request.Version);

        await contractTemplateRepository.AddContractTemplateAsync(template, cancellationToken);
    }
}
