using MediatR;

namespace UserService.Application.Features.Users.GetUserForContract;

public record GetUserForContractQuery(Guid UserId) : IRequest<ClientForContractResponse>;