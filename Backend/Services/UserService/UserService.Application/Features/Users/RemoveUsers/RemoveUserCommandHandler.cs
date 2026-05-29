using MediatR;
using UserService.Application.Authorization;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.RemoveUsers;

public class RemoveUserCommandHandler(
    IUserRepository userRepository) : 
    IRequestHandler<RemoveUserCommand>
{
    public async Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        var userToRemove = await userRepository.GetByIdAsync(request.Id, cancellationToken) 
                           ?? throw new UserNotFoundException("User not found");
        
        await userRepository.RemoveAsync(userToRemove, cancellationToken);
    }
}