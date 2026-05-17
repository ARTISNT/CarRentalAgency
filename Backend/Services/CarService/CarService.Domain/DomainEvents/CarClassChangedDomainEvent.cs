using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarClassChangedDomainEvent(Guid Id, CarClass NewCarClass, DateTime OccurredOn) : IDomainEvent;