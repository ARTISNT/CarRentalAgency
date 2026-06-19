namespace Contracts.RentalEvents;

public record RentalReturnRequestedIntegrationEvent(
    Guid RentalId,
    Guid UserId,
    string UserEmail,
    DateTime RequestedAt,
    decimal CostAtRequestTime);
