using MediatR;
using UserService.Application.Authorization;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.DeactivateUser;

public class DeactivateUserCommandHandler(
    IUserAuthorizationService userAuthorizationService,
    IUserRepository userRepository) 
    : IRequestHandler<DeactivateUserCommand>
{
    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken) 
                   ?? throw new UserNotFoundException("User not found");

        await userAuthorizationService.EnsureCanDeactivateUser(request.Id, cancellationToken);
        user.Deactivate();
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}