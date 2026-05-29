using MediatR;

namespace UserService.Application.Features.Users.DeactivateUser;

public record DeactivateUserCommand(Guid Id) : IRequest;