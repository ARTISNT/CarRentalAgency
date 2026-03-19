using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.Events;

public record UserPasswordChangedDomainEvent(User User) :  INotification;