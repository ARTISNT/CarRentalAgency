using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarRepaintedDomainEvent(Guid Id, Color NewColor, DateTime OccurredOn) : IDomainEvent;