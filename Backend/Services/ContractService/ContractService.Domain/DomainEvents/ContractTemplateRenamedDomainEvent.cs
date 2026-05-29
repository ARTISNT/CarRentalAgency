using ContractService.Domain.Common;

namespace ContractService.Domain.DomainEvents;

public record ContractTemplateRenamedDomainEvent(Guid Id, string NewName, DateTime OccuredOn) : IDomainEvent;