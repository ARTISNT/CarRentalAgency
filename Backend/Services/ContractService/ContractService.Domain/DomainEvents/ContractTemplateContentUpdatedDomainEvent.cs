using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractTemplateContentUpdatedDomainEvent(Guid Id, string NewContent, DateTime OccuredOn) : IDomainEvent;