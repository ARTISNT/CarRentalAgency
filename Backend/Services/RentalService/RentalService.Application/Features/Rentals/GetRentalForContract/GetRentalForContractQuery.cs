using Contracts.Common;
using MediatR;

namespace RentalService.Application.Features.Rentals.GetRentalForContract;

public record GetRentalForContractQuery(Guid Id) : IRequest<RentalForContractResponse>, IAuthorizedRequest;