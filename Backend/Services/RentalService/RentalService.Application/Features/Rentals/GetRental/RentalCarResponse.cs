namespace RentalService.Application.Features.Rentals.GetRental;

public class RentalCarResponse
{
    public string Model { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string? Generation { get; set; }

    public string? Variant { get; set; }

    public bool IsFacelift { get; set; }

    public string LicensePlate { get; set; } = null!;

    public decimal PricePerHour { get; set; }

    public string CarClass { get; set; } = null!;
}