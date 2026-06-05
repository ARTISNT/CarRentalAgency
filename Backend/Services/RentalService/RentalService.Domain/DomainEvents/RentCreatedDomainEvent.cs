using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentCreatedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;