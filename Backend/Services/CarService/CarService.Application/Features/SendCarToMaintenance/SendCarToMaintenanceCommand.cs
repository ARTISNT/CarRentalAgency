using MediatR;

namespace CarService.Application.Features.SendCarToMaintenance;

public record SendCarToMaintenanceCommand(Guid CarId) : IRequest;
