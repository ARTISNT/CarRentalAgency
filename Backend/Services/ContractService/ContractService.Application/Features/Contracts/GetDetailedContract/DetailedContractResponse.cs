namespace ContractService.Application.Features.Contracts.GetDetailedContract;

public class DetailedContractResponse
{
    public Guid Id { get; init; }

    public Guid ContractTemplateId { get; init; }

    public string Status { get; init; }

    public string PdfPath { get; init; }

    public ClientResponse Client { get; init; }

    public ContractAutoResponse Car { get; init; }

    public ContractTemplateResponse Template { get; init; }

    public RentalResponse Rental { get; init; }
}