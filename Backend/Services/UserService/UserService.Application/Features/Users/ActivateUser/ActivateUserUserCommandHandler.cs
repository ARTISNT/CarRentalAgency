using MediatR;
using UserService.Application.Authorization;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.ActivateUser;

public class ActivateUserUserCommandHandler(
    IUserRepository userRepository,
    IUserAuthorizationService userAuthorizationService) 
    : IRequestHandler<ActivateUserCommand>
{
    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken) 
                   ?? throw new UserNotFoundException("User not found");

        await userAuthorizationService.EnsureCanActivateUser(request.Id, cancellationToken);
        user.Activate();
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}