using MediatR;

namespace UserService.Application.Features.Users.ActivateUser;

public record ActivateUserCommand(Guid Id) : IRequest;
