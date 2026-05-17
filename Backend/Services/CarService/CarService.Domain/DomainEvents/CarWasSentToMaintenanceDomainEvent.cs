using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarWasSentToMaintenanceDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;