namespace Contracts.ContractEvents;

public record ContractCreatedIntegrationEvent(
    Guid ContractId,
    Guid ClientId,
    Guid RentalId,
    DateTime CreatedAt);
