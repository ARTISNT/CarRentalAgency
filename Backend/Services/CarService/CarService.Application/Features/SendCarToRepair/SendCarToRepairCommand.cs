using MediatR;

namespace CarService.Application.Features.SendCarToRepair;

public record SendCarToRepairCommand(Guid CarId) : IRequest;
