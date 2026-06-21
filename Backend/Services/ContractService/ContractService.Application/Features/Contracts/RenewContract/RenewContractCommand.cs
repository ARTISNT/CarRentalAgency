using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.Contracts.RenewContract;

public record RenewContractCommand(Guid RentalId, decimal AdditionalPrice, DateTime NewEndDate) : IRequest, IAuthorizedRequest;