namespace ContractService.Application.Features.Contracts.GetDetailedContract;

public class RentalResponse
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal EstimatedPrice { get; init; } 
}