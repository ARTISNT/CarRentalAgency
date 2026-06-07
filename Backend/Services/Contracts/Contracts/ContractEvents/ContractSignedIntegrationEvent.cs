namespace Contracts.ContractEvents;

public record ContractSignedIntegrationEvent(
    Guid ContractId,
    Guid ClientId,
    Guid RentalId,
    DateTime SignedAt);
