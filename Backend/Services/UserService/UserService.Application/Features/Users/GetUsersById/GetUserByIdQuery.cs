using Contracts.Common;
using MediatR;
using UserService.Application.Common;
using UserService.Application.Features.Users.GetUsers;

namespace UserService.Application.Features.Users.GetUsersById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserResponse>, IAuthorizedRequest;