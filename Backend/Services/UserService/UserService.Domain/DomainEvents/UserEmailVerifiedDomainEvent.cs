using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserEmailVerifiedDomainEvent(User User) : INotification;