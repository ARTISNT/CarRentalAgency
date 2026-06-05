namespace Contracts.RentalEvents;

public record RentalEndedIntegrationEvent(
    Guid RentalId,
    Guid UserId,
    DateTime ReturnDate,
    decimal TotalCost);
