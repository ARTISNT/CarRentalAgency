namespace ContractService.Application.Features.Contracts.CreateContract;

public class RentalForContractResponse
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal TotalPrice { get; init; }
}