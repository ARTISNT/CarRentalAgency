namespace Contracts.ContractEvents;

public record ContractEndedIntegrationEvent(
    Guid ContractId,
    Guid RentalId,
    Guid CarId,
    Guid ClientId,
    int Mileage,
    decimal FuelLevel,
    decimal PenaltyAmount,
    string? DamageDescription,
    DateTime EndedAt);
