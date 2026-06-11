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
    public string LicensePlate { get; set; } = null!;
    public string VinCode { get; set; } = null!;
    public string? Color { get; set; }
    public double HorsePower { get; set; }
    public double? EngineVolume { get; set; }
    public double? PowerReverse { get; set; }
    public double? FuelCurrentLiters { get; set; }
    public double? FuelCapacityLiters { get; set; }
    public double? BatteryCurrentKWh { get; set; }
    public double? BatteryCapacityKWh { get; set; }
}
