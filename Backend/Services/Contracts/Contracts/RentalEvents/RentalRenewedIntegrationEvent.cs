namespace Contracts.RentalEvents;

public record RentalRenewedIntegrationEvent(Guid Id, Guid UserId, DateTime NewEndDate, decimal AdditionalPrice);
