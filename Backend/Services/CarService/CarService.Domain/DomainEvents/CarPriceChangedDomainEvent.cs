using CarService.Domain.Cars.ValueObjects;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarPriceChangedDomainEvent(Guid Id, PricePerHour NewPricePerHour, DateTime OccurredOn) : IDomainEvent;