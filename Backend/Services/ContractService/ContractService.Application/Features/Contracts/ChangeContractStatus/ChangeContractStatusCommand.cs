using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.Contracts.ChangeContractStatus;

public record ChangeContractStatusCommand(Guid ContractId, string NewStatus) : IRequest, IAuthorizedRequest;