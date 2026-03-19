using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.Events;

public record UserEmailVerifiedDomainEvent(User User) : INotification;