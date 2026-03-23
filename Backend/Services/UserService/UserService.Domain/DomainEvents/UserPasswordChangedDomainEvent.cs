using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.DomainEvents;

public record UserPasswordChangedDomainEvent(User User) :  INotification;