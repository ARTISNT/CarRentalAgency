using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarBecameAvailableDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;