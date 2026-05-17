using MediatR;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUsers;

public record GetUsersQuery() : IRequest<IReadOnlyCollection<UserResponse>>;