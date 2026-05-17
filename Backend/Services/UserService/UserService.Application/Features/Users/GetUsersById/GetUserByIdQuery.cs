using MediatR;
using UserService.Application.Features.Users.GetUsers;

namespace UserService.Application.Features.Users.GetUsersById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserResponse>;