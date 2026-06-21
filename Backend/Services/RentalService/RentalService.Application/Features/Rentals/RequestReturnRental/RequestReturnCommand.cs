using Contracts.Common;
using MediatR;

namespace RentalService.Application.Features.Rentals.RequestReturnRental;

public record RequestReturnCommand(Guid Id) : IRequest, IAuthorizedRequest;
