using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Domain.Common;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.RegisterUser;

public class RegisterUserCommandHandler(IUserRepository userRepository, 
    ILogger<RegisterUserCommandHandler> logger, IPasswordProcessor passwordProcessor) 
    : IRequestHandler<RegisterUserCommand, Guid>
{
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var phoneNumber = new PhoneNumber(request.PhoneNumber);
        var email = new Email(request.Email);
        var password = Password.Create(request.Password, passwordProcessor);
        
        var user = new User(phoneNumber, email, password);
        
        logger.LogInformation("Register user - {@User}", user);
        await userRepository.AddAsync(user, cancellationToken);
        
        return user.Id;
    }
}