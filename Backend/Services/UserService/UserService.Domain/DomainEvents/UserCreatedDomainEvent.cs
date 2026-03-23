using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserCreatedDomainEvent(User User) : INotification;
