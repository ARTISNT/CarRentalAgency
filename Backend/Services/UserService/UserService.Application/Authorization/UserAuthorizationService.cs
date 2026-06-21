using UserService.Application.Common;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Authorization;

public class UserAuthorizationService(
    IUserContext userContext,
    IUserRepository userRepository)
    : IUserAuthorizationService
{
    public async Task EnsureCanViewUser(
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await GetCurrentUser(cancellationToken);

        if (!currentUser.CanView(targetUserId))
            throw new ForbiddenException("No permission");
    }

    public async Task EnsureCanEditUser(
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await GetCurrentUser(cancellationToken);

        if (!currentUser.CanEdit(targetUserId))
            throw new ForbiddenException("No permission");
    }

    public async Task EnsureCanDeactivateUser(
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await GetCurrentUser(cancellationToken);

        if (!currentUser.CanDeactivate(targetUserId))
            throw new ForbiddenException("No permission");
    }

    public async Task EnsureCanActivateUser(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await GetCurrentUser(cancellationToken);

        if (!currentUser.CanActivate(targetUserId))
            throw new ForbiddenException("No permission");
    }

    private async Task<User> GetCurrentUser(CancellationToken cancellationToken)
    {
        if (userContext.UserId is not Guid currentUserId)
            throw new UserNotFoundException("Current user not found");

        return await userRepository.GetByIdAsync(currentUserId, cancellationToken)
               ?? throw new UserNotFoundException("Current user not found");
    }
}