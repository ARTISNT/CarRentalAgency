namespace RentalService.Api.Requests;

public class CreateRentalRequest
{
    public Guid UserId { get; set; }
    public Guid CarId { get; set; }
    public DateTime StartDate { get; set; } 
    public DateTime EndDate { get; set; }
    public string? PromoCode { get; set; }
}