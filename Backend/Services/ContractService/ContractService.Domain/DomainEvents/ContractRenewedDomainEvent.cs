using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractRenewedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;