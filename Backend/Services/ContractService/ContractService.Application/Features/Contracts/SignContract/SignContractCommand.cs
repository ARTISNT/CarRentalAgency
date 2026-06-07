using MediatR;

namespace ContractService.Application.Features.Contracts.SignContract;

public record SignContractCommand(Guid Id, string SignatureBase64) : IRequest;