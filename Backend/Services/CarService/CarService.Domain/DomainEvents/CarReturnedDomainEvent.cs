using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarReturnedDomainEvent(Guid CarId, DateTime OccurredOn) : IDomainEvent;
