using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetDetailedContract;

public record GetDetailedContractQuery(Guid Id) : IRequest<DetailedContractResponse>, IAuthorizedRequest;