namespace RentalService.Api.Requests;

public class EndRentalRequest
{
    public string? PromoCode { get; set; } = null;
    public DateTime ReturnDate { get; set; }
}