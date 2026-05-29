using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractTemplateCreatedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;