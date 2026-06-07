using MediatR;

namespace CarService.Application.Features.CompleteMaintenance;

public record CompleteMaintenanceCommand(Guid CarId) : IRequest;
