using MediatR;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUsers;

public record GetUsersQuery(UserSpecification UserSpecification) : IRequest<IReadOnlyCollection<UserResponse>>;