using UserService.Domain.Common;

namespace UserService.Domain.DomainEvents;

public record UserPasswordChangedDomainEvent(Guid Id, string NewPasswordHash, DateTime OccurredOn) : IDomainEvent;