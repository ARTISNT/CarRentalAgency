using MediatR;

namespace ContractService.Application.Features.Contracts.CreateContract;

public record CreateContractCommand(Guid? ClientId, Guid RentalId, Guid CarId, Guid ContractTemplateId) : IRequest;