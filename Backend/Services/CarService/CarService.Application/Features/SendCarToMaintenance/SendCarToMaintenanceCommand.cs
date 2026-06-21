using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.SendCarToMaintenance;

public record SendCarToMaintenanceCommand(Guid CarId) : IRequest, IAuthorizedRequest;
