using CarService.Application.Abstractions.Security;
using Contracts.Common;
using MediatR;

namespace CarService.Application.Common;

public class AuthorizationBehavior<TRequest, TResponse>(
    IClientContext clientContext) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IAuthorizedRequest && clientContext.IsActive == false)
            throw new AccountDeactivatedException();

        return await next(cancellationToken);
    }
}
