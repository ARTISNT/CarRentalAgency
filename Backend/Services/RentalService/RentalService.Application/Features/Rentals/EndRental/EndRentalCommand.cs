using Contracts.Common;
using MediatR;

namespace RentalService.Application.Features.Rentals.EndRental;

public record EndRentalCommand(
    Guid Id,
    DateTime ReturnDate,
    int Mileage,
    decimal FuelLevel,
    decimal PenaltyAmount,
    string? DamageDescription) : IRequest, IAuthorizedRequest;