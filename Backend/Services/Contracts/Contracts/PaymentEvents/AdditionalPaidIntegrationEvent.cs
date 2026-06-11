namespace Contracts.PaymentEvents;

public record AdditionalPaidIntegrationEvent(
    Guid RentalId,
    Guid TransactionId,
    decimal Amount,
    DateTime PaidAt);
