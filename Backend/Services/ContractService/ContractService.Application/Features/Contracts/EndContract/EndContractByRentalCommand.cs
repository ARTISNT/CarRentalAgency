using MediatR;

namespace ContractService.Application.Features.Contracts.EndContract;

public record EndContractByRentalCommand(
    Guid RentalId,
    DateTime ReturnDate,
    int Mileage,
    decimal FuelLevel,
    decimal PenaltyAmount,
    string? DamageDescription) : IRequest;
