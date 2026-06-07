using MediatR;

namespace ContractService.Application.Features.Contracts.CancelContract;

public record CancelContractByRentalCommand(Guid RentalId, string? Reason) : IRequest;
