using MediatR;

namespace UserService.Application.Features.Users.LoginUser;

public record LoginUserQuery(LoginUserRequest LoginUserRequest) : IRequest<string>;