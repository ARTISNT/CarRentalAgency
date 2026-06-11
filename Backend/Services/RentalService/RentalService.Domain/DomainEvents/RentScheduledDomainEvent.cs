using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentScheduledDomainEvent(Guid Id, DateTime StartDate, DateTime OccuredOn) : IDomainEvent;
