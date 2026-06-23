using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.Contracts.CancelContract;

public record CancelContractCommand(Guid ContractId, string? Reason = null) : IRequest, IAuthorizedRequest;