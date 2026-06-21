using Contracts.Common;
using ContractService.Application.Abstractions.Security;
using MediatR;

namespace ContractService.Application.Common;

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
