using CarService.Domain.Cars.ValueObjects;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarModelInfoUpdatedDomainEvent(Guid Id, CarModelInfo ModelInfo, DateTime OccurredOn) : IDomainEvent;