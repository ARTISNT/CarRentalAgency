using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarCreatedDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;