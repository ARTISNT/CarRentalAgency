namespace Contracts.PaymentEvents;

public record FineChargedIntegrationEvent(
    Guid RentalId,
    decimal Amount,
    string Reason,
    DateTime ChargedAt);
