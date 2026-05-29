namespace RentalService.Application.Features.Rentals.GetRentalForContract;

public class RentalForContractResponse
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal TotalPrice { get; init; }
}