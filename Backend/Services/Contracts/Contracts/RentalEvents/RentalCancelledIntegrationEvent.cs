namespace Contracts.RentalEvents;

public record RentalCancelledIntegrationEvent(
    Guid RentalId,
    Guid CarId,
    Guid UserId,
    DateTime CancelledAt,
    string? Reason);
