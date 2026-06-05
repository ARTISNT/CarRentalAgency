namespace RentalService.Application.Features.Rentals.GetRentalForContract;

public class RentalForContractResponse
{
    public Guid RentalId { get; set; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal EstimatedPrice { get; init; }
}