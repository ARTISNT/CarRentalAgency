using Contracts.Common;
using MediatR;

namespace RentalService.Application.Features.Rentals.RenewRental;

public record RenewRentalCommand(Guid Id, DateTime NewDate) : IRequest, IAuthorizedRequest;