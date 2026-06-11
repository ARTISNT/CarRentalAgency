namespace Contracts.RentalEvents;

public record RentalRenewedIntegrationEvent(Guid Id, Guid UserId, string UserEmail, DateTime NewEndDate, decimal AdditionalPrice);
