using Contracts.Common;
using MediatR;
using UserService.Application.Common;
using UserService.Domain.Users;

namespace UserService.Application.Common;

public class AuthorizationBehavior<TRequest, TResponse>(
    IUserContext userContext,
    IUserRepository userRepository) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IAuthorizedRequest)
            return await next(cancellationToken);

        if (!userContext.IsUserRequest)
            return await next(cancellationToken);

        var user = await userRepository.GetByIdAsync(userContext.UserId!.Value, cancellationToken);

        if (user is not null && !user.IsActive)
            throw new AccountDeactivatedException();

        return await next(cancellationToken);
    }
}
