using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.Common;
using UserService.Domain.Common;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.RegisterUser;

public class RegisterUserCommandHandler(
    IUserRepository userRepository, 
    ILogger<RegisterUserCommandHandler> logger,
    IJwtProvider  jwtProvider,
    IPasswordProcessor passwordProcessor) 
    : IRequestHandler<RegisterUserCommand, string>
{
    public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var phoneNumber = new PhoneNumber(request.PhoneNumber);
        var email = new Email(request.Email);
        var password = Password.Create(request.Password, passwordProcessor);
        
        var user = new User(phoneNumber, email, password);
        string jwt = jwtProvider.CreateJwtToken(user);
        
        logger.LogInformation("Register user - {@User}", user);
        await userRepository.AddAsync(user, cancellationToken);
        
        return jwt;
    }
}