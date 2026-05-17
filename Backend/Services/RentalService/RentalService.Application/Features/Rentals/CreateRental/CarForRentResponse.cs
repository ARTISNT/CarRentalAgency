namespace RentalService.Application.Features.Rentals.CreateRental;

public class CarForRentResponse
{
    public string Model { get; set; }
    public string Brand { get; set; }
    public string? Generation { get; set; }
    public string? Variant  { get; set; }
    public bool IsFacelift  { get; set; }
    public string LicensePlate  { get; set; }
    public string AvailabilityStatus  { get; set; }
    public decimal PricePerHour   { get; set; }
    public string CarClass   { get; set; }
}
