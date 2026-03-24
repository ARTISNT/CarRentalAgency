using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserEmailVerifiedDomainEvent(Guid Id, DateTime OccuredOn ) : INotification;