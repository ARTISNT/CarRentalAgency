using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.ProcessReturnedCar;

public record ProcessReturnedCarCommand(Guid CarId, string TargetStatus) : IRequest, IAuthorizedRequest;
