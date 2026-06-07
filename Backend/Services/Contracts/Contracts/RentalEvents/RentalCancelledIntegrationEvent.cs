namespace Contracts.RentalEvents;

public record RentalCancelledIntegrationEvent(
    Guid RentalId,
    DateTime CancelledAt,
    string? Reason);
