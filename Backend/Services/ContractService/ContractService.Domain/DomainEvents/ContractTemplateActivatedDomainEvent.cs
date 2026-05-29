using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractTemplateActivatedDomainEvent(Guid Id, bool NewStatus, DateTime OccuredOn) : IDomainEvent;