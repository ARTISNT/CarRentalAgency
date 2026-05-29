namespace ContractService.Application.Features.Contracts.GetContracts;

public class ContractListResponse
{
    public Guid Id { get; init; }

    public string ClientFullName { get; init; }

    public string Car { get; init; }

    public DateTime StartDate { get; init; }

    public DateTime EndDate { get; init; }

    public decimal TotalPrice { get; init; }

    public string Status { get; init; }

    public string PdfPath { get; init; }    
}