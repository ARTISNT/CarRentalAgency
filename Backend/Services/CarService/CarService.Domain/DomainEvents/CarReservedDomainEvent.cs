using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarReservedDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;
