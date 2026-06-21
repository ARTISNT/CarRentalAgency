using Contracts.Common;
using MediatR;

namespace RentalService.Application.Features.Rentals.GetRental;

public record GetRentalQuery(Guid Id) : IRequest<RentalResponse>, IAuthorizedRequest;