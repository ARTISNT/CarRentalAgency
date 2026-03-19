using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserActivatedDomainEvent(User User) : INotification;