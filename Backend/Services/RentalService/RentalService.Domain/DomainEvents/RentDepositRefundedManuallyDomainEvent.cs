using RentalService.Domain.Common;

namespace RentalService.Domain.DomainEvents;

public record RentDepositRefundedManuallyDomainEvent(
    Guid Id,
    DateTime RefundedAt,
    string? Note,
    DateTime OccuredOn) : IDomainEvent;
