using MediatR;
using UserService.Domain.Users;

namespace UserService.Domain.Events;

public class UserCreatedDomainEvent(User user) : INotification;
