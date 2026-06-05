using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractCancelledDomainEvent(Guid Id, string Reason, DateTime OccuredOn) : IDomainEvent;