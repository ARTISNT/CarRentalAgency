namespace Contracts.ContractEvents;

public record ContractSignedIntegrationEvent(
    Guid ContractId,
    Guid ClientId,
    DateTime SignedAt);
