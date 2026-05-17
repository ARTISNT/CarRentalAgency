using UserService.Domain.Common;

namespace UserService.Domain.DomainEvents;

public record UserEmailVerifiedDomainEvent(Guid Id, DateTime OccurredOn ) : IDomainEvent;