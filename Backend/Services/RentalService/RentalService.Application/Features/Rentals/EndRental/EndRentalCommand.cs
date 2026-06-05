using MediatR;

namespace RentalService.Application.Features.Rentals.EndRental;

public record EndRentalCommand(Guid Id, DateTime ReturnDate) : IRequest;