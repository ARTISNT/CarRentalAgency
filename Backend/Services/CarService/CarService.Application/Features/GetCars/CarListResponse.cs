namespace CarService.Application.Features.GetCars;

public class CarListResponse
{
    public Guid Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public decimal PricePerHour { get; set; }
    public string Class { get; set; }
    public string? Generation { get; set; }
    public bool IsFacelift { get; set; }
    public string? Variant { get; set; }
    public string AvailabilityStatus { get; set; }
    public string Status { get; set; }
    public string PhotoUrl { get; set; }
}
