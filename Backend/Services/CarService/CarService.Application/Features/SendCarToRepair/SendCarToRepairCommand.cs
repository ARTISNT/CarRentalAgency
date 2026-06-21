using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.SendCarToRepair;

public record SendCarToRepairCommand(Guid CarId) : IRequest, IAuthorizedRequest;
