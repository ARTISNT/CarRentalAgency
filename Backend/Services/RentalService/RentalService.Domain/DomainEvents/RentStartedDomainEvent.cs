using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentStartedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;