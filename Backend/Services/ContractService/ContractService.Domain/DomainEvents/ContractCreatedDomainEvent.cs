using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractCreatedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;