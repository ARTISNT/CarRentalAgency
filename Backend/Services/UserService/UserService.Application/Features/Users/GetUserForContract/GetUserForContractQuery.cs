using Contracts.Common;
using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Users.GetUserForContract;

public record GetUserForContractQuery(Guid UserId) : IRequest<ClientForContractResponse>, IAuthorizedRequest;