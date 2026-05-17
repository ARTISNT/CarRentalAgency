using CarService.Domain.Cars.ValueObjects;
using CarService.Domain.Common;

namespace CarService.Domain.DomainEvents;

public record CarLicensePlateUpdatedDomainEvent(Guid Id, LicensePlate LicensePlate, DateTime OccurredOn) : IDomainEvent;