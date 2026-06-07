namespace Contracts.RentalEvents;

public record RentalStartedIntegrationEvent(
    Guid RentalId,
    Guid UserId,
    Guid CarId,
    DateTime StartedAt);
