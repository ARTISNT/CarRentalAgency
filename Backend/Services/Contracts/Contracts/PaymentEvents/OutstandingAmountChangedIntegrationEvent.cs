namespace Contracts.PaymentEvents;

public record OutstandingAmountChangedIntegrationEvent(
    Guid RentalId,
    decimal RequiredAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    string Reason);
