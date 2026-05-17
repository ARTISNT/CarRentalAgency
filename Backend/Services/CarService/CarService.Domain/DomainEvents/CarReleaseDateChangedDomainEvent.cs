using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarReleaseDateChangedDomainEvent(Guid Id, DateTime NewReleaseDate, DateTime OccurredOn) : IDomainEvent;