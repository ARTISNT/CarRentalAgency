namespace RentalService.Api.Requests;

public class EndRentalRequest
{
    public DateTime ReturnDate { get; set; }
    public int Mileage { get; set; }
    public decimal FuelLevel { get; set; }
    public decimal PenaltyAmount { get; set; }
    public string? DamageDescription { get; set; }
}