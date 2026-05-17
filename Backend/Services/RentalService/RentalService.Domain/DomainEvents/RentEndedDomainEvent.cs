using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentEndedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;