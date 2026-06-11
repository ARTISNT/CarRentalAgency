namespace Contracts.RentalEvents;

public record RentalCreatedIntegrationEvent(
    Guid RentalId,
    Guid UserId,
    string UserEmail,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate,
    decimal EstimatedCost);
