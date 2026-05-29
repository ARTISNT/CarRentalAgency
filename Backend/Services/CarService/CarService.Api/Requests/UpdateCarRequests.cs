namespace CarService.Api.Requests;

public sealed record UpdateCarRequests
{
    public DateTime ReleaseDate { get; init; }

    public string LicensePlate { get; init; } = default!;
    public string VinCode { get; init; } = default!;

    public string Color { get; init; } = default!;

    // Model info
    public string Model { get; init; } = default!;

    public string Brand { get; init; } = default!;

    public string? Generation { get; init; }

    public bool IsFacelift { get; init; }

    public string? Variant { get; init; }

    // Technical info
    public double Mileage { get; init; }

    public string BodyStyle { get; init; } = default!;

    public string TransmissionType { get; init; } = default!;

    public string DriveType { get; init; } = default!;

    // Engine
    public string EngineType { get; init; } = default!;

    public double EngineVolume { get; init; }

    public int HorsePower { get; init; }
    public double PowerReverse { get; init; }

// Pricing
    public double PricePerHour { get; init; }

    // Class
    public string CarClass { get; init; } = default!;

    // Photo
    public string PhotoUrl { get; init; } = default!;
}