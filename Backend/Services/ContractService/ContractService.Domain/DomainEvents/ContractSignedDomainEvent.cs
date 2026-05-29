using ContractService.Domain.Common;
using ContractService.Domain.Contracts;

namespace ContractService.Domain.DomainEvents;

public record ContractSignedDomainEvent(Guid Id, ContractStatus NewStatus, DateTime OccuredOn) : IDomainEvent;