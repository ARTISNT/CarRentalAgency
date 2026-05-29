using MediatR;

namespace UserService.Application.Features.Users.LoginUser;

public record LoginUserQuery(string Email, string Password) : IRequest<string>;