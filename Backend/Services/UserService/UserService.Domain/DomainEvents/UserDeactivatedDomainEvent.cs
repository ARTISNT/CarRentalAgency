using UserService.Domain.Common;

namespace UserService.Domain.DomainEvents;

public record UserDeactivatedDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;
