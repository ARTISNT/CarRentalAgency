using RentalService.Domain.Common;

namespace RentalService.Domain.Rentals;

public record RentCarSnapshot(
    string Model,
    string Brand,
    string? Generation,
    string? Variant,
    bool IsFacelift,
    string LicensePlate,
    string AvailabilityStatus,
    decimal PricePerHour,
    string CarClass) : IValueObject;