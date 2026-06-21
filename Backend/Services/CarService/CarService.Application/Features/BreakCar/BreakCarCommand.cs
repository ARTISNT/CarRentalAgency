using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.BreakCar;

public record BreakCarCommand(Guid CarId) : IRequest, IAuthorizedRequest;
