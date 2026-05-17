using MediatR;

namespace RentalService.Application.Features.Rentals.CancelRental;

public record CancelRentalCommand(Guid Id, DateTime CancelledAt, string? Reason = null) : IRequest;