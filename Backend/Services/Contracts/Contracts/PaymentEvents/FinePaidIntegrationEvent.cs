namespace Contracts.PaymentEvents;

public record FinePaidIntegrationEvent(
    Guid RentalId,
    Guid TransactionId,
    decimal Amount,
    DateTime PaidAt);
