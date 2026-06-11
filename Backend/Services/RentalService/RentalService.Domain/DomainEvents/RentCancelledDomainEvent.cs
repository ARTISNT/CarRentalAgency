using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentCancelledDomainEvent(Guid Id, DateTime CancelledAt, DateTime OccuredOn, string? Reason = null) : IDomainEvent;