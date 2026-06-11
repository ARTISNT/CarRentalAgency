namespace Contracts.PaymentEvents;

public record DepositPaidIntegrationEvent(
    Guid RentalId,
    DateTime PaidAt,
    string PaymentTypeName = "Deposit",
    decimal Amount = 0);
