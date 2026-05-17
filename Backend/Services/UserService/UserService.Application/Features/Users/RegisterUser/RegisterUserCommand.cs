using MediatR;

namespace UserService.Application.Features.Users.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string PhoneNumber) : IRequest<Guid>;