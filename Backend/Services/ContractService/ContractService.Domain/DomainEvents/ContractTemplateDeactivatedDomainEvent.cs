using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractTemplateDeactivatedDomainEvent(Guid Id, bool NewStatus, DateTime OccuredOn) : IDomainEvent;