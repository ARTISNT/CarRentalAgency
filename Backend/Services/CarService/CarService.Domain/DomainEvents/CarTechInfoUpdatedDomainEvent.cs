using CarService.Domain.Cars.ValueObjects;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarTechInfoUpdatedDomainEvent(Guid Id, CarTechInfo TechInfo, DateTime OccurredOn) : IDomainEvent;
    