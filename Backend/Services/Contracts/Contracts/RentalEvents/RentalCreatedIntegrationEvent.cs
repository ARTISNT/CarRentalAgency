namespace Contracts.RentalEvents;

public record RentalCreatedIntegrationEvent(
    Guid RentalId,
    Guid UserId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate,
    decimal EstimatedCost);
