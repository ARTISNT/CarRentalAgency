using UserService.Domain.Common;

namespace UserService.Domain.DomainEvents;

public record UserActivatedDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;