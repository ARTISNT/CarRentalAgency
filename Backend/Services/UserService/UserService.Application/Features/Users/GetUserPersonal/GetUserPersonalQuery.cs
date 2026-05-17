using MediatR;
using UserService.Application.Features.Users.GetUsers;

namespace UserService.Application.Features.Users.GetUserPersonal;

public record GetUserPersonalQuery(Guid Id) : IRequest<UserResponseWithPassport>;