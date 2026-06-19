using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentReturnRequestedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;
