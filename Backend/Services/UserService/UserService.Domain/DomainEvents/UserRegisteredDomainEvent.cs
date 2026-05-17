using UserService.Domain.Common;

namespace UserService.Domain.DomainEvents;

public record UserRegisteredDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;