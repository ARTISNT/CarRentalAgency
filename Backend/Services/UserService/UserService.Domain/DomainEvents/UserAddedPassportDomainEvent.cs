using UserService.Domain.Common;

namespace UserService.Domain.DomainEvents;

public record UserAddedPassportDomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent;