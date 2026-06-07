namespace Contracts.PaymentEvents;

public record DepositPaidIntegrationEvent(Guid RentalId, DateTime PaidAt);
