namespace Contracts.RentalEvents;

public record RentalEndedIntegrationEvent(
    Guid RentalId,
    Guid UserId,
    DateTime ReturnDate,
    decimal TotalCost,
    int Mileage = 0,
    decimal FuelLevel = 0,
    decimal PenaltyAmount = 0,
    string? DamageDescription = null);
