using MediatR;

namespace ContractService.Application.Features.Contracts.EndContract;

public record EndContractByRentalCommand(
    Guid RentalId,
    int Mileage,
    decimal FuelLevel,
    decimal PenaltyAmount,
    string? DamageDescription) : IRequest;
