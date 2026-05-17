using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentRenewedDomainEvent(Guid Id, DateTime OccuredOn) : IDomainEvent;