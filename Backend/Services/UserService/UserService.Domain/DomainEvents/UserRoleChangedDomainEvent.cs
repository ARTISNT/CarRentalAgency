using UserService.Domain.Common;

namespace UserService.Domain.DomainEvents;

public record UserRoleChangedDomainEvent(Guid Id, string NewRole, DateTime OccurredOn) : IDomainEvent;