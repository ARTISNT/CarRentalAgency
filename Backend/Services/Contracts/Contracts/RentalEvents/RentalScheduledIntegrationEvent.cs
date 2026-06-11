namespace Contracts.RentalEvents;

public record RentalScheduledIntegrationEvent(
    Guid RentalId,
    Guid UserId,
    Guid CarId,
    DateTime ScheduledAt);
