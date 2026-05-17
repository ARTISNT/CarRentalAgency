using MediatR;

namespace RentalService.Application.Features.Rentals.CreateRental;

public record CreateRentalCommand(Guid UserId, Guid CarId, DateTime StartDate, DateTime EndDate) : IRequest<Guid>;