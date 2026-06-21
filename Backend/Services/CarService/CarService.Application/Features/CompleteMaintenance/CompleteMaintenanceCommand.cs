using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.CompleteMaintenance;

public record CompleteMaintenanceCommand(Guid CarId) : IRequest, IAuthorizedRequest;
